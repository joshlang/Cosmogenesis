# Cosmogenesis

Cosmogenesis is a C# source generator for CosmosDB.

You define some documents, sprinkle in a couple [attribute]s, and Cosmogenesis spits out all the code to use it in CosmosDB.

## Installation

...Haven't figured out how to package it as a nuget package yet.  Until then, you can grab the repo.

## Usage

### CosmosDB Setup

Your CosmosDB container must have the following properties:
- Partition key = `/pk`
- Check the checkbox saying `My partition key is larger than 100 bytes`

### Project Setup

Create a `C#` `.net standard 2.1` `class library`

Add a reference to `Cosmogenesis.Core`

Add a source generator reference to `Cosmogenesis.Generator`.  There's no point-and-click way to do this yet in visual studio, so open up your .csproj file and add:

```
<ItemGroup>
  <ProjectReference Include="[path-to]/Cosmogenesis.Generator[.csproj or .dll]" ReferenceOutputAssembly="false" OutputItemType="Analyzer" />
</ItemGroup>
```

You should be using nullable reference types.  If not, you are a dinosaur and should use visual basic instead. ;D

Define some documents (these are from `Cosmogenesis.TestDb4`):
```
[Partition("Accounts")]
public abstract class AccountDocBase : DbDoc
{
    [PartitionDefinition("Accounts")]
    public static string GetPk(Guid accountId) => $"Account={accountId:N}";

    public Guid AccountId { get; set; }
}

[Mutable]
public sealed class AccountInfoDoc : AccountDocBase
{
    public static string GetId() => "Info";

    public string Name { get; set; } = default!;
    public bool IsEvil { get; set; }
    public int MinionCount { get; set; }
}

[Transient]
[Mutable]
public sealed class PhoneNumberDoc : AccountDocBase
{
    public static string GetId(string phoneNumberType) => $"PhoneNumberType={phoneNumberType}";

    public string PhoneNumberType { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public bool IsActive { get; set; }
}

[Partition("Orders")]
public sealed class OrderDoc : DbDoc
{
    [PartitionDefinition("Orders")]
    public static string GetPk(Guid accountId) => $"Orders={accountId:N}";
    public static string GetId(string orderNumber) => $"Order={orderNumber}";

    public Guid AccountId { get; set; } = default!;
    public string OrderNumber { get; set; } = default!;

    public class Item
    {
        public string ItemCode { get; set; } = default!;
        public decimal UnitCost { get; set; }
        public long Quantity { get; set; }
    }

    public Item[] Items { get; set; } = default!;
    public List<string> Notes { get; set; } = default!;
    public decimal TotalPrice { get; set; }
}
```

Now, in some other app, you can add a reference to the project you just created.

Here is some sample code (from `Cosmogenesis.TestDb4.App`) so you get a flavor of how it gets consumed:

```
var cosmosClient = new CosmosClient("a connection string");
var container = cosmosClient.GetDatabase("a database name").GetContainer("a container name");

var db = new EvilCorpDb(container); // using Evil.Corp.Database;
await db.ValidateContainerAsync();

var acctId = Guid.NewGuid();

// Create a single AccountInfo document
var accountInfo = await db
    .Partition
    .Accounts(accountId: acctId)
    .Create
    .AccountInfoDocAsync(name: "Bob", isEvil: true, minionCount: 4, accountId: acctId)
    .ThrowOnConflict();  // using Cosmogenesis.Core;


// Update minion count and add two PhoneNumber documents, atomically in a batch
++accountInfo.MinionCount;
await db
    .Partition
    .Accounts(accountId: acctId) // We could have saved this value from before
    .CreateBatch()
    .CreatePhoneNumberDoc(phoneNumberType: "Mobile", phoneNumber: "555-Evil", isActive: true, accountId: acctId)
    .CreatePhoneNumberDoc(phoneNumberType: "Land", phoneNumber: "556-Evil", isActive: true, accountId: acctId)
    .Replace(accountInfo)
    .ExecuteOrThrowAsync(); // Explode if the batch fails
// The accountInfo variable is now "stale" and must be reloaded if more operations on it are needed
// To avoid this, we could have used ExecuteWithResultsAsync to get the new version of accountInfo

            
// Load any "Mobile" phone number for this account
var mobile = await db
    .Partition
    .Accounts(accountId: acctId)
    .Read
    .PhoneNumberDocAsync(phoneNumberType: "Mobile");

            
// And delete it if we found one
if (mobile is not null)
{
    await db
        .Partition
        .Accounts(accountId: acctId)
        .DeleteAsync(mobile)
        .ThrowOnConflict();
}


// Create an order for the account
await db
    .Partition
    .Orders(accountId: acctId)
    .Create
    .OrderDocAsync(
        accountId: acctId,
        orderNumber: DateTime.Now.ToString(),
        items: new[]
        {
            new OrderDoc.Item { ItemCode = "abc", Quantity = 1, UnitCost = 1.99m },
            new OrderDoc.Item { ItemCode = "def", Quantity = 100, UnitCost = 999 }
        },
        notes: new List<string>() { "This person is pretty evil", "Be careful!" },
        totalPrice: 99901.99m)
    .ThrowOnConflict();


// Load all "Land" phone numbers that are active for all accounts
var activeLandPhoneNumbers = await db
    .CrossPartitionQuery
    .PhoneNumberDocs(x => x.Where(p => p.IsActive && p.PhoneNumberType == "Land"))
    .ToListAsync();  // Install nuget package System.Linq.Async for this
```

Here's what the `OrderDoc` looks like inside CosmosDB, which the code above created:

```
{
    "AccountId": "7a8ca455-7346-4d85-ab01-28890e733b92",
    "OrderNumber": "3/19/2021 1:44:13 PM",
    "Items": [
        {
            "ItemCode": "abc",
            "UnitCost": 1.99,
            "Quantity": "1"
        },
        {
            "ItemCode": "def",
            "UnitCost": 999,
            "Quantity": "100"
        }
    ],
    "Notes": [
        "This person is pretty evil",
        "Be careful!"
    ],
    "TotalPrice": 99901.99,
    "pk": "Orders=7a8ca45573464d85ab0128890e733b92",
    "id": "Order=3_19_2021 1:44:13 PM",
    "_etag": "\"9b00a55d-0000-0a00-0000-60550d1c0000\"",
    "Type": "OrderDoc",
    "CreationDate": "2021-03-19T20:44:13.1272959Z",
    "_rid": "WWZSAPAwiFIEAAAAAAAAAA==",
    "_self": "dbs/WWZSAA==/colls/WWZSAPAwiFI=/docs/WWZSAPAwiFIEAAAAAAAAAA==/",
    "_attachments": "attachments/",
    "_ts": 1616186652
}
```

## Important Change Information

**USE NAMED PARAMETERS**!!!

The source generators do not guarantee the same parameter ordering.  Use named parameters to avoid nasty gotchas.

Example:  `Something(bool isOk, bool willDieImmediately);` without named parameters would be called like `Something(true, false)`.  Without warning, the source generator might change it to `Something(bool willDieImmediately, bool isOk);`, in which case your parameters are backwards.  Using `Something(isOk: true, willDieImmediately: false)` will always work without problems.

Also: Don't rename document classes or methods unless you really know what you're doing.  And since I haven't really documented everything and it's all magic, good luck with that :D

## Motivations

I was just playing around.  I saw that source generators were "improved" in one of the visual studio preview release notes and wanted to experiment with them.

This was adapted from an old source generator I made (using reflection and physical .cs files).  I wanted to see if the experience was better using the new C# source generation magic.

It's much better :D

I think database generation tools are a pretty epic use-case for C# source generators.

## Feedback to VS and C# Devs

Hey Visual Studio developers.  You're awesome!  And I'm pretty sure you'll make source generation scenarios awesome too... but we're clearly not quite there yet!

### Biggest Ask

Show the generated .cs files!  Hide it under the "Show all files" toggle or something

Why?  Because once I was able to generate .cs files, there were obviously compile-time errors showing.  But there was no way to see the files I had generated, to find and fix the errors.  F12 only works if compilation worked.

My workaround:
- Set EmitCompilerGeneratedFiles to true (see the .csproj file for the TestDb projects)
- But they don't show up?
- Toggle "Show all files"
- They're still not there!
- Search using explorer.  Oh, there they are under `/obj/...`
- Open them in visual studio
- Syntax highlighting kinda works, but kinda doesn't, and errors don't show up
- Create "Temp" folder in the project and move generated files there
- **Now everything works!**
- Must delete the "Temp" folder before rebuild, and repeat the process

### Second Biggest Ask

A better debugging experience

I'm new to roslyn stuff (syntax and symbols, as opposed to reflection).  So the debugger was crucial for me feeling my way through all this.

My approach was to add `Debugger.Launch();` in my generator then rebuild the target project.  Of course, during the rebuild, a prompt opens and lets me start the debugger, and I can walk my way through.  But of course, it's cumbersome to add/remove it all the time.  And removing it *is* often necessary to stop the debugger from popping up after every keystroke.

An F5 experience somehow would be ideal.  Maybe a button that says "Build [with generator debugger]" as a menu option.

### State?

Can I save state between generations?

Scenario: if someone renames "BobDoc" to "BobDoc2", how could I detect the change and warn that they're about to destabalize the schema?

### To Generate or Not To Generate?

Where's the line between what should be generated and what should sit in a "normal" dependency?

I have seen several samples and tutorials which "inject" an attribute via `GeneratorInitializationContext.RegisterForPostInitialization`.  Cosmogenesis also uses attributes, but they just sit within the `Cosmogenesis.Core` project.

It will be interesting to watch best practices being developed to answer: Should these attributes be generated, or stay where they are?

Pros to leaving the attributes in the project (aka "no generation"):
- There's no code customization needed (the code for the attributes won't ever change based on someone else's code)
- Easier to create (no generation code required)
- Easier to examine (they're right there in a project)
- Consistent identity (other stuff might look for the attributes via reflection)

Pros to generation:
- The attributes *only* exist for code generation purposes and have no effect on `Cosmogenesis.Core` itself
- They can be internal and won't clutter up any namespace
- Can't foresee any reason why outsiders might want to examine the attributes

I've considered moving them to be generated.  If I do, it will be for one main reason:  The attributes only exist for code generation purposes.

The initial decision to put them in the project may have been driven by my unfamiliarity with source generation.

### Testing?

How should generated code be tested?  Should I also generate unit testing code?  But since that's code too, I'm pretty sure a testing stack overflow happens somewhere.

Anyway, I'm not sure what approach to take with testing.  Looking forward to see what best practices emerge.

### Usage Analyzers?

See the notes above regarding **using named parameters**.  It would be nice to have some sort of analyzer which could look at how the generated code is being used and raise warnings.

This might just be a regular analyzer.  I've never made an analyzer.  But if so, I suppose I'd have to convince people to use it as part of how Cosmogenesis gets consumed, and that I can't just force it on people who are using the generator.

Not sure how I feel about this yet.

### Roslyn Education

Simple things like "how do I know if this symbol is of type X" ended up not being so simple.

I think a bit of education on common approaches would speed up adoption of code generators, since Roslyn stuff is integral to it all.  Right now, it feels a bit too advanced to be 100% comfortable.  Probably like how I felt back in my first foray into reflection.

### Existing Information

There exist some resources which served well to get me to understand the gist of how they work.  They were basically my "Hello World" samples.
- [C# Source Generators - Write Code that Writes Code](https://www.youtube.com/watch?v=3YwwdoRg2F4)
- [Introducing C# Source Generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/)
- [Better C# - Source Generators](https://www.youtube.com/watch?v=1u33UTdllV0)

## License

You must own at least 1 Dogecoin to use this project in production.

