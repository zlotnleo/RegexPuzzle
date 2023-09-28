We are working with objects that I will call _regexes_. A regex is a representation of a set of strings that it _matches_. The rules for what a regex is and the strings that it matches are as follows:
* Any of the 26 lowercase alphabetic characters is a valid regex. The only string that it matches is the length-1 string consisting of that character and nothing else.
* If `R` is a regex, then `(R)*` (brackets mandatory) is a regex. It matches any string which is the concatenation of a (possibly empty) sequence of strings, each of which is matched by `R`.
* If `R` is a regex, then `(R)+` (brackets mandatory) is a regex. It matches any string which is the concatenation of a non-empty sequence of strings, each of which is matched by `R`.
* If `R` is a regex, then `(R)?` (brackets mandatory) is a regex. It matches any string which is either empty or matched by `R`.
* If `R1` and `R2` are regexes, then `(R1|R2)` (brackets mandatory) is a regex. It matches any string which is matched by either `R1` or `R2`.
* If `R1` and `R2` are regexes, then `R1R2` is a regex. It matches any string which is the concatenation of a string matched by `R1` and a string matched by `R2`.

The file `regexes.txt` consists of some regexes, one per line. Can you find a single string that is matched by all of them?