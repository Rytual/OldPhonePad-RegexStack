# OldPhonePad-RegexStack: Pattern Matching Meets Data Structures

```
┌──────────────────────────────────────────────────────────┐
│  Finding Patterns Like Finding Your Ex's Number         │
│       in Your Call History                              │
│                                                          │
│  Input: "222 333#"                                       │
│     │                                                    │
│     ▼                                                    │
│  ┌──────────────┐                                        │
│  │ Regex Match  │ Pattern: (\d)\1*                      │
│  │  "222"  ->  Key '2' x 3 = 'C'                        │
│  │  " "    ->  (skip)                                   │
│  │  "333"  ->  Key '3' x 3 = 'F'                        │
│  └──────┬───────┘                                        │
│         │                                                │
│         ▼                                                │
│  ┌──────────────┐                                        │
│  │    Stack     │                                        │
│  │  Push 'C'    │  Stack: [C]                           │
│  │  Push 'F'    │  Stack: [C, F]                        │
│  └──────┬───────┘                                        │
│         │                                                │
│         ▼                                                │
│     Result: "CF"                                         │
└──────────────────────────────────────────────────────────┘
```

## The Challenge

This approach combines two concepts - regex for pattern matching and Stack for handling backspaces. Instead of manually counting characters, let regex find consecutive digit sequences. Instead of string manipulation for backspaces, use a stack where backspace is just a Pop().

The keypad layout:

```
1: &'(        2: abc       3: def
4: ghi        5: jkl       6: mno
7: pqrs       8: tuv       9: wxyz
*: backspace  0: space     #: send
```

### Examples

- `33#` → `E`
- `227*#` → `B` (type CA, backspace)
- `4433555 555666#` → `HELLO`
- `8 88777444666*664#` → `TURING`

## My Approach

I combined regex preprocessing with stack-based result building:

**Regex Pattern Matching:**
```csharp
// Matches consecutive identical digits
private static readonly Regex consecutiveDigitsPattern =
    new Regex(@"(\d)\1*", RegexOptions.Compiled);
```
The pattern `(\d)\1*` captures a digit and matches any repeats. So "222" matches as one group.

**Stack for Backspaces:**
```csharp
var resultStack = new Stack<char>();

// Add character? Push it
resultStack.Push('A');

// Backspace? Pop it
if (resultStack.Count > 0)
    resultStack.Pop();
```

**What works well:**
- Regex handles pattern matching cleanly
- Stack makes backspace operations O(1)
- Clear separation of concerns
- No string manipulation overhead

**What's a bit complex:**
- Regex has a learning curve
- Need to reverse the stack at the end (LIFO)
- Might be overkill for simple inputs

Works okay when you need pattern recognition or have frequent backspaces. The other implementations are simpler if you don't need these features.

## Getting Started

### Prerequisites

- .NET 8.0 or later

### Running the Code

```bash
# Clone the repository
git clone https://github.com/yourusername/OldPhonePad-RegexStack.git
cd OldPhonePad-RegexStack

# Build and test
dotnet build
dotnet test

# For verbose test output
dotnet test --logger "console;verbosity=detailed"
```

### Using the Decoder

```csharp
using OldPhonePad.RegexStack;

// Simple decoding
string result = OldPhonePad.OldPhonePad("33#");
Console.WriteLine(result); // Output: E

// With backspaces
result = OldPhonePad.OldPhonePad("227*#");
Console.WriteLine(result); // Output: B
```

### Analyzing Patterns (Debug Helper)

```csharp
string analysis = OldPhonePad.AnalyzeInput("222 333#");
Console.WriteLine(analysis);

// Shows how regex parsed the input
// Useful for debugging
```

## How It Works

**Step 1:** Validate input format with regex

**Step 2:** Use regex to match consecutive digit sequences
- Input "222 33" matches as: "222" (key 2, count 3), "33" (key 3, count 2)

**Step 3:** Push decoded characters onto stack

**Step 4:** Handle backspaces with Pop()

**Step 5:** Convert stack to string (reversing for correct order)

## Test Coverage

The project has 50+ tests covering:
- All provided examples
- Edge cases (empty input, backspaces, spaces)
- Key press combinations
- Backspace operations
- Special keys
- Complex scenarios
- Error handling
- Regex-specific tests (pattern matching, consecutive digits)
- Stack-specific tests (push/pop behavior, reversal)

## Implementation Details

**Regex Patterns:**
- `(\d)\1*` - Matches consecutive identical digits
- `^[0-9\s\*]*#$` - Validates input format

**Stack Operations:**
- Push: Add character to result
- Pop: Remove character (backspace)
- Reverse: Convert LIFO stack to string

**Performance:**
- Regex matching: O(n)
- Stack operations: O(1)
- String conversion: O(m)
- Overall: O(n + m)

## Project Structure

```
OldPhonePad-RegexStack/
├── src/
│   ├── OldPhonePad.cs                   # Regex + Stack decoder
│   └── OldPhonePad.RegexStack.csproj
├── tests/
│   ├── OldPhonePadTests.cs              # Test suite
│   └── OldPhonePad.RegexStack.Tests.csproj
├── .github/
│   └── workflows/
│       └── dotnet.yml                    # CI/CD
├── .gitignore
├── LICENSE
└── README.md
```

## Other Implementations

Check out the other approaches:
- **OldPhonePad-DictionaryState**: Simple dictionary with manual state tracking
- **OldPhonePad-FSM**: Finite state machine with formal state transitions
- **OldPhonePad-Grouping**: Groups consecutive digits before processing
- **OldPhonePad-OOP**: Object-oriented design with separate classes

Each has different tradeoffs.

## Fun Note

The regex pattern took a bit to get right - backreferences (`\1`) aren't something you use every day. The stack part was straightforward but I initially forgot to reverse it at the end. LIFO means last in, first out, so you need to flip the order when converting to a string. Debugging with the AnalyzeInput method helped a lot.

## License

MIT License - see LICENSE file for details.

---

*Built for the Iron Software coding challenge - October 2025*
