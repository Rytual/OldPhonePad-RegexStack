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

[![.NET Build](https://github.com/ironsoftware/OldPhonePad-RegexStack/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ironsoftware/OldPhonePad-RegexStack/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

> *Remember when you'd scroll through your call history looking for that one number you forgot to save? You'd scan for patterns—repeated digits, familiar sequences. That's basically what regex does, but faster and without the emotional baggage.*

## The Philosophy: Pattern Recognition + Elegant Undo

This implementation combines two powerful concepts:
1. **Regex preprocessing** to identify and group consecutive key presses
2. **Stack data structure** for elegant backspace handling

Instead of manually counting characters, we let regex do the heavy lifting of pattern matching. And instead of awkwardly removing characters from strings, we use a stack where backspace is just a simple `Pop()` operation.

## Key Features

### 1. Regex-Powered Pattern Matching

We use a compiled regex pattern to identify consecutive digit sequences:

```csharp
// Matches one or more consecutive occurrences of the same digit
private static readonly Regex consecutiveDigitsPattern =
    new Regex(@"(\d)\1*", RegexOptions.Compiled);
```

This pattern is simple:
- `(\d)` - Captures a digit into group 1
- `\1*` - Matches zero or more occurrences of that same digit (backreference)

So "222" matches as one sequence, "33" matches as another and we process them as units.

### 2. Stack-Based Result Building

Instead of string concatenation or StringBuilder, we use a Stack:

```csharp
var resultStack = new Stack<char>();

// Adding a character? Push it!
resultStack.Push('A');

// Backspace? Pop it!
if (resultStack.Count > 0)
    resultStack.Pop();
```

This is nice because:
- Backspace is O(1) instead of O(n) for string manipulation
- Stack naturally models the "undo" operation
- LIFO behavior matches how backspace works

### 3. Functional Pipeline Style

The algorithm flows like a data pipeline:
```
Input String
  → Regex Matching
    → Stack Operations
      → String Conversion
        → Result
```

## Installation

```bash
git clone https://github.com/ironsoftware/OldPhonePad-RegexStack.git
cd OldPhonePad-RegexStack
dotnet build
dotnet test
```

## Usage

### Basic Usage

```csharp
using OldPhonePad.RegexStack;

// Simple decoding
string result = OldPhonePad.OldPhonePad("33#");
Console.WriteLine(result); // Output: E

// More complex message
result = OldPhonePad.OldPhonePad("4433555 555666#");
Console.WriteLine(result); // Output: HELLO

// With backspaces
result = OldPhonePad.OldPhonePad("227*#");
Console.WriteLine(result); // Output: B
```

### Analyzing Regex Patterns (Debug Helper)

This implementation includes a special method to see how the regex parsing works:

```csharp
string analysis = OldPhonePad.AnalyzeInput("222 333#");
Console.WriteLine(analysis);

// Output:
// Input Analysis:
// Original: 222 333#
//
// Consecutive Digit Matches:
//   Position 0: '222' -> Key '2' pressed 3 time(s) -> 'C'
//   Position 3: ' ' -> (skipped)
//   Position 4: '333' -> Key '3' pressed 3 time(s) -> 'F'
```

This is useful for:
- Understanding how the regex engine processes your input
- Debugging complex sequences
- Learning how pattern matching works
- Showing off your regex knowledge

## How It Works: Deep Dive

### Step 1: Validation with Regex

Before processing, we validate the input format:

```csharp
private static readonly Regex validInputPattern =
    new Regex(@"^[0-9\s\*]*#$", RegexOptions.Compiled);
```

This checks the input contains only:
- Digits (0-9)
- Spaces (for pauses)
- Asterisks (for backspace)
- Must end with # (send)

### Step 2: Regex Pattern Matching

We iterate through the input and use regex to match consecutive digit sequences:

```csharp
Match match = consecutiveDigitsPattern.Match(sequence, i);

if (match.Success && match.Index == i)
{
    char key = match.Groups[1].Value[0];  // The digit
    int pressCount = match.Length;         // How many times

    // Process this key press
}
```

For input "222 33", regex finds:
- Match 1: "222" (key '2', count 3)
- Match 2: "33" (key '3', count 2)

### Step 3: Stack Operations

Each matched sequence gets translated to a character and pushed onto the stack:

```csharp
if (character.HasValue)
{
    resultStack.Push(character.Value);
}
```

Backspaces simply pop from the stack:

```csharp
if (currentChar == '*')
{
    if (resultStack.Count > 0)
        resultStack.Pop();
}
```

### Step 4: Stack to String Conversion

Finally, we convert the stack to a string (remembering to reverse since stack is LIFO):

```csharp
private static string StackToString(Stack<char> stack)
{
    var chars = new char[stack.Count];
    int index = stack.Count - 1;

    foreach (char c in stack)
        chars[index--] = c;

    return new string(chars);
}
```

## Running the Tests

We have 50+ tests including regex-specific test cases:

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run only regex-specific tests
dotnet test --filter "FullyQualifiedName~Regex"
```

### Test Categories

Our tests cover:
- **Basic Functionality**: Single/multiple key presses
- **Edge Cases**: Empty input, excessive backspaces
- **Error Handling**: Null input, invalid format
- **Complex Scenarios**: Long messages, mixed patterns
- **Regex Patterns**: Consecutive matching, pattern parsing
- **Stack Behavior**: Push/pop operations, backspace handling

## Performance Characteristics

### Time Complexity
- **Regex Matching**: O(n) where n is input length
- **Stack Operations**: O(1) for push/pop
- **String Conversion**: O(m) where m is result length
- **Overall**: O(n + m), typically O(n)

### Space Complexity
- **Stack Storage**: O(m) where m is result length
- **Regex Matches**: O(k) where k is number of digit sequences
- **Overall**: O(m + k), typically O(m)

### Regex Compilation
Using `RegexOptions.Compiled` means:
- Slower first call (JIT compilation)
- Faster subsequent calls (native code)
- Perfect for repeated use

## Advantages of This Approach

**Pattern Recognition Power**
Regex excels at finding patterns. Consecutive digits are a pattern. Let regex do what it does best.

**Clean Backspace Handling**
Stack makes backspace trivial:
- No substring operations
- No string rebuilding
- No off-by-one errors
- Just `Pop()`

**Clear Intent**
The code reads like the problem statement:
```csharp
// Find consecutive digits
Match match = consecutiveDigitsPattern.Match(sequence, i);

// Process them
char? character = GetCharacterForKey(key, pressCount);

// Build result
resultStack.Push(character.Value);
```

**Debugging Visibility**
The `AnalyzeInput()` method lets you see exactly what the regex is doing - useful for understanding and debugging.

**Performance**
- Compiled regex is fast
- Stack operations are O(1)
- No repeated string allocations

## Disadvantages (Because Honesty Matters)

**Regex Learning Curve**
Not everyone is comfortable with regex. `(\d)\1*` might look like line noise to beginners.

**Regex Overhead**
For very simple inputs, regex might be overkill. The compilation and pattern matching have overhead.

**Stack Reversal Required**
Converting stack to string requires reversing the order, adding a small O(m) operation.

**Less Intuitive for Some**
Some developers find explicit loops more readable than regex patterns.

## When to Use This Approach

**Good for:**
- Input with complex patterns
- When you need to analyze/debug the parsing process
- Applications that process many messages (compiled regex pays off)
- When backspace operations are frequent
- Learning about regex and data structures

**Overkill for:**
- Very simple, short inputs
- One-off processing
- Teams without much regex expertise
- Environments where regex library isn't available

## Nostalgic Tech Corner: Pattern Matching Through the Ages

Remember when pattern matching meant:
- Trying to remember your crush's phone number pattern (was it 555-0123 or 555-0132?)
- Looking for patterns in Snake scores to beat your high score
- Memorizing button combos for special moves (←↓→ + Punch)
- Finding your old conversations by scrolling and looking for that distinctive pattern of "u there?"

Regex is kind of like that, but computerized. It's pattern matching with superpowers:

```regex
\d{3}-\d{4}     # Phone number pattern
(\d)\1+         # Repeated digits (like when you held a key too long)
[A-Z]{2,}       # ALL CAPS MESSAGES (because you felt strongly about it)
```

The Stack data structure is equally nostalgic. It's like that pile of paper on your desk:
- Put new stuff on top (push)
- Take the top thing off (pop)
- Never dig through the middle (unless you're desperate)

In our phone keypad decoder, regex finds the patterns and stack handles the undo operations. Like having a pattern-recognition AI and a time machine working together. Except it's 2025 and we're using them to decode text from phone technology from the 1990s. Full circle.

## Regex Patterns Explained

Let's break down the key patterns used:

### Consecutive Digits Pattern: `(\d)\1*`
```regex
(\d)    # Capture a digit into group 1
\1*     # Match zero or more occurrences of that same digit
```

Examples:
- "222" → Matches "222" (digit 2, three times)
- "33" → Matches "33" (digit 3, twice)
- "2 3" → Matches "2", then space breaks it, then matches "3"

### Input Validation Pattern: `^[0-9\s\*]*#$`
```regex
^           # Start of string
[0-9\s\*]*  # Zero or more: digits, spaces, or asterisks
#           # Must end with hash
$           # End of string
```

This checks input is formatted right before we process it.

## Related Repositories

This is part of a collection exploring different approaches to the same problem:

- [OldPhonePad-Simple](https://github.com/ironsoftware/OldPhonePad-Simple) - The straightforward approach
- [OldPhonePad-Functional](https://github.com/ironsoftware/OldPhonePad-Functional) - Pure functional approach
- [OldPhonePad-StateMachine](https://github.com/ironsoftware/OldPhonePad-StateMachine) - Finite state machine implementation
- [OldPhonePad-OOP](https://github.com/ironsoftware/OldPhonePad-OOP) - Object-oriented design
- **OldPhonePad-RegexStack** - You are here! (Regex + Stack)

Each repository demonstrates different programming paradigms and trade-offs. Good for:
- Learning different problem-solving approaches
- Comparing performance characteristics
- Understanding when to use which technique
- Interview preparation
- Showing off your skills

## Contributing

Got a regex pattern that works better? Found a way to optimize the stack operations? PRs welcome!

Make sure:
- All tests pass (`dotnet test`)
- Code follows C# conventions
- Regex patterns are commented and explained
- You've added tests for new functionality

## License

MIT License - See [LICENSE](LICENSE) file for details.

---

*Built with regex magic, stack elegance, and fond memories of when finding patterns in your call history felt like detective work.*

**Pro tip**: Understanding regex is like understanding why your ex texted "k" vs "ok" vs "okay" - it's all about the patterns. Master the patterns, master the universe. Or at least master this phone keypad decoder.

**Remember**: Regex is powerful but with great power comes great confusion when you look at your own regex six months later. Comment your patterns, future you will thank you.
