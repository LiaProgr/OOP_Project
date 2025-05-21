
using System.Text.Json;
using System.IO;
using EventTickets;
public static class EventRepository
{
    private static List<Event> _events=new();
    private const string FilePath = "eventss.json";

    //властивість для private _events*
    public static List<Event> Events
    {
        get
        {
            if (_events == null)
            {
                LoadEvents();
            }
            return _events;
        }
    }

    //редагування події не зовсім*
    public static void UpdateEvent(Event updatedEvent)
    {
        var existing = _events.FirstOrDefault(e => e.Name == updatedEvent.Name);
        if (existing != null)
        {
            existing.AvailableTickets = updatedEvent.AvailableTickets;
            existing.ReservedTickets = updatedEvent.ReservedTickets;
            SaveEvents();//ЗБ
        }
    }
    //додавння події до головного списку Напряму*
    public static void AddEvent(Event newEvent)
    {
        Events.Add(newEvent);
        //SaveEvents();//ЗБ
    }
    //видалення події з головного списку Напряму*
    public static void RemoveEvent(Event eventToRemove)
    {
        Events.Remove(eventToRemove);
        //SaveEvents();//ЗБ
    }
  //  завантаження подій з файлу*
    public static void LoadEvents()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                _events = JsonSerializer.Deserialize<List<Event>>(json) ?? new List<Event>();
            }
            catch
            {
                _events = new List<Event>();

            }
        }
        else
        {
            _events = new List<Event>();

        }
    }
    //збереження подій у файл*
    public static void SaveEvents()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(Events, options);
        File.WriteAllText(FilePath, json);
    }
}