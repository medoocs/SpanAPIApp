namespace SpanAPI
{
    public static class Extensions
    {
        public static PersonDto AsPersonDto(this Person person)
        {
            return new PersonDto(person.Name, person.LastName, person.PostalCode, person.City, person.Number);
        }
        public static Person AsPersonModel(this PersonDto personDto)
        {
            return new Person(personDto.Name, personDto.LastName, personDto.PostalCode, personDto.City, personDto.Number);
        }

        public static PersonCsvDto AsPersonCsvDto(this Person person)
        {
            return new PersonCsvDto(person.Name, person.LastName, person.PostalCode.ToString(), person.City, person.Number, true);
        }
        public static Person AsModel(this PersonCsvDto personCsvDto)
        {
            return new Person(personCsvDto.Name, personCsvDto.LastName, Int32.Parse(personCsvDto.PostalCode), personCsvDto.City, personCsvDto.Number);
        }
    }
}
