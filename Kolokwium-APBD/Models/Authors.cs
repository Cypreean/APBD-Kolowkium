using System.ComponentModel.DataAnnotations;

public class Authors
{
    public Authors(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Surname { get; set; }
}