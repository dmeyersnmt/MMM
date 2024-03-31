


using MMDev;
using MMTools;


foreach (var settings in Settings.metric_names)
{
    Settings.metric_name = settings;
    Entry _entry = new Entry();
}

CalculateScore calculateScore = new CalculateScore("MMDEV");