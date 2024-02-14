using UnityEngine;
using System.Collections;

public enum CharacterClass
{
    Elf,
    Angel
}

public class CharacterAsset : ScriptableObject 
{
	public CharacterClass charClass;
	public string charName;
	[TextArea(2,3)] public string description;
	public int maxHealth = 30;
    public int attack;
	public Sprite charIllustration;
	public Sprite previewCharIllustration;

	// public string HeroPowerName;
    // public Sprite HeroPowerIconImage;
}
