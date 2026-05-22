namespace SoraMod.SoraModCode.Synergy;

public interface IDriveFormSynergy
{
    // The power will call this when the form activates or the card is drawn
    void ApplyDriveSynergy(); 
    
    // The power will call this when the form ends
    void RemoveDriveSynergy(); 
}