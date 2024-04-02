using MMDev;
using MMTools;
using NLog;


Logger logger = LogManager.GetCurrentClassLogger();
logger.Info("Start");
logger.Info(@$"Settings:  
                year_run:{Settings.year_run} 
                model_iterations: {Settings.model_iterates}");


foreach (var settings in Settings.metric_names)
{
    Settings.metric_name = settings;
    Entry _entry = new Entry();
}

//This is the new and improved "judger" function
CalculateScore calculateScore = new CalculateScore("MMDEV");

logger.Info("Finished");