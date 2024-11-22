namespace VeryLargeTextFile.Generator;

public class Record(
    int number, 
    string text
    )
{
    public int Number { get; } = number;
    public string Text { get; set; } = text;

    public override string ToString() => $"{Number}. {Text}";
    
}