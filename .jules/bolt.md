# Bolt's Journal

## 2024-05-22 - [First Entry]
**Learning:** Initial setup of Bolt's journal.
**Action:** Always check for this file and maintain it.

## 2024-05-22 - [Manual JSON Construction Anti-Pattern]
**Learning:** The codebase heavily uses manual string concatenation to build JSON responses (O(N^2) in loops). When refactoring to serialization, BE CAREFUL with data types. Legacy code might quote numbers (e.g. `"min": "10"`), while serializers output numbers (e.g. `"min": 10`). This breaks API contracts.
**Action:** Always verify JSON output format matches exactly, including quotes around numbers if that was the legacy behavior.
