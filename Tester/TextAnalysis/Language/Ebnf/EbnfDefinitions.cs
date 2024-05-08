namespace ModularSystem.Core.TextAnalysis.Language.Ebnf;

public static class EbnfDefinitions
{
    public const string JsonEbnf = @"
(* A JSON text is a serialized value. *)
json        = element ;

(* Whitespace can appear between any pair of tokens. *)
ws          = *( %x20 / %x09 / %x0A / %x0D ) ;

(* A value can be an object, array, number, string, or one of the following three literal names. *)
value       = object / array / number / string / ""true"" / ""false"" / ""null"" ;

element     = ws value ws ;

(* Objects are an unordered collection of zero or more name/value pairs. *)
object      = ""{"" ws [ member *( "","" member ) ] ws ""}"" ;
member      = string ws "":"" element ;

(* Arrays are an ordered collection of zero or more values. *)
array       = ""["" ws [ element *( "","" element ) ] ws ""]"" ;

(* Numbers can be integers or floating point and include exponents. *)
number      = [ ""-"" ] int [ frac ] [ exp ] ;
int         = ""0"" / digit1-9 *digit ;
frac        = ""."" 1*digit ;
exp         = ( ""e"" / ""E"" ) [ ""-"" / ""+"" ] 1*digit ;

(* Strings are a sequence of zero or more Unicode characters. *)
string      = %x22 *( char / escape ) %x22 ; (* Quotation mark ("") wraps a string of characters or escapes *)
char        = unescaped /
              %x20-21 / %x23-5B / %x5D-10FFFF ; (* All valid unicode characters except "" or \ or control characters *)
escape      = ""\\"" ( ""u"" 4HEXDIG /            (* u followed by four hexadecimal digits *)
                      %x22 /                  (* Quotation mark ("") *)
                      %x5C /                  (* Reverse solidus (\) *)
                      ""b"" /                   (* Backspace *)
                      ""f"" /                   (* Form feed *)
                      ""n"" /                   (* Newline *)
                      ""r"" /                   (* Carriage return *)
                      ""t"" ) ;                 (* Horizontal tab *)

(* Helpers for number production *)
digit1-9   = %x31-39 ;                        (* Digits 1 through 9 *)
digit      = %x30 / digit1-9 ;                (* Digits 0 through 9 *)
HEXDIG     = digit / %x41-46 / %x61-66 ;      (* Hexadecimal digit (0-9, A-F, a-f) *)
";

}


