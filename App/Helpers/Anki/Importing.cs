namespace Nihongo.App.Helpers.Anki;

/*
IMPORTING INFO
https://docs.ankiweb.net/importing.html

	- The import files must be plain text (myfile.txt)
	- File must be in UTF-8 ('UTF-8 encoding' for japanese and other non-latin chars)
	- Anki determines the number of fieldsand separating character 
		by looking at the first (non-commented) line
	- To include HTML formatting, the "allow HTML in fields" checkbox exists 
	- To include quotes inside field, "escape" them
		replacing a single doublequote (") with 2 doublequotes ("")
	- By default, the existing note’s other fields will be updated
		based on content of duplicate from the imported file
	- # this is a comment and is ignored
	- Headers (#key:value pairs) must be listed in separate lines at the top of the file
		-- separator
		-- html:	true, false
		-- guid column
		-- columns
		-- notetype, notetype column
		-- deck, deck column		
*/

public class Importing
{
    public static List<string> FileHeader(Deck deck, NoteType note)
    {
        return new List<string> {
            $"#deck:\"{deck.ToString()}\"",
            $"#notetype:\"{note.ToString()}\"",
            "#html:true"
        };
    }

    public static string OutputPathJoyo(int grade)
    {
        return $@"{Environment.CurrentDirectory}\anki_import_jōyō_grade{(grade > 6 ? 8 : grade)}.txt";
    }
}