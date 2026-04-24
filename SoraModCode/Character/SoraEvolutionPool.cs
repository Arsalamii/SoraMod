using BaseLib.Abstracts;
using Godot;
using SoraMod.SoraModCode.Extensions;

namespace SoraMod.SoraModCode.Character;

// A hidden pool to safely store our evolutions without them dropping as rewards!
public class SoraEvolutionPool : CustomCardPoolModel
{
    // A unique internal ID so it doesn't clash with Sora's main pool
    public override string Title => "SoraEvolutions"; 

    // THE FIX: Tells the engine to register this pool even though Sora doesn't "own" it!
    public override bool IsShared => true;

    // --- VISUALS COPY/PASTED FROM SORAMODCARDPOOL ---
    // This ensures Fira and Cura look exactly like normal Sora cards!
    
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    public override float H => 1f; 
    public override float S => 1f; 
    public override float V => 1f; 

    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;
}