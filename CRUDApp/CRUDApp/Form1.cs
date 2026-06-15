using System.Collections.Generic;

// Модель данных
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
}

// Класс для работы с данными
public class DataManager
{
    private List<Person> _people = new List<Person>();
    private int _currentId = 1;

    public List<Person> GetAllPeople() => _people;

    public void AddPerson(Person person)
    {
        person.Id = _currentId++;
        _people.Add(person);
    }

    public void UpdatePerson(Person person)
    {
        var existingPerson = _people.FirstOrDefault(p => p.Id == person.Id);
        if (existingPerson != null)
        {
            existingPerson.Name = person.Name;
            existingPerson.Surname = person.Surname;
            existingPerson.Age = person.Age;
        }
    }

    public void DeletePerson(int id)
    {
        _people.RemoveAll(p => p.Id == id);
    }

    public List<Person> FilterPeople(string searchText)
    {
        return _people.Where(p =>
            p.Name.Contains(searchText) ||
            p.Surname.Contains(searchText)).ToList();
    }
}
