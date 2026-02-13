# NOTES
*.http is used with the rest client extension
use record instead of regular class because record is immutable, and compares obj by value not memory references. eg p1 == p2 = true if p1.name == p2.name