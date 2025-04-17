using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameCharacter : MonoBehaviour{

    public Combat c;
    public Transform bars;

    // STATS
    string cName;
    int hp;
    int vitality;
    int armor;
    int strength;
    int magic;
    int mana;
    int maxMana;
    public Bar healthBar;
    public Bar manaBar;

    public string CName{get { return cName; } set {this.cName = value; }}
    public int HP{get{ return hp; } set{ this.hp = value; }}
    public int Vitality{ get { return Mathf.RoundToInt((vitality + GetEquipmentSum(0)) * GetEquipmentMult(0)); } set{ this.vitality = value; }}
    public int Armor{ get { return Mathf.RoundToInt((armor + GetEquipmentSum(1)) * GetEquipmentMult(1)); } set { this.armor = value; }}
    public int Strength{get { return Mathf.RoundToInt((strength + GetEquipmentSum(2)) * GetEquipmentMult(2)); } set{ this.strength = value; }}
    public int Magic{get { return Mathf.RoundToInt((magic + GetEquipmentSum(3)) * GetEquipmentMult(3)); } set{ this.magic = value; }}
    public int Mana{get{ return Mathf.RoundToInt((mana + GetEquipmentSum(4)) * GetEquipmentMult(4)); } set{ this.mana = value; }}
    public int MaxMana{get{ return maxMana; } set {this.maxMana = value; }}

    // SKILLS
    public Skill[] skills;
    public List<Skill> unlockedSkills = new List<Skill>();

    int skillCount;
    public int SkillCount{ get{ return skillCount; }}

    int selectedSkill = 0;

    // INVENTORY
    public Equipment equipment;
    public Item[] inventory;

    // to change sprite
    SpriteManager spriteManager;
    public SpriteManager SM{ get { return spriteManager; }}
    Transform moveCharacterSprite;
    public List<Sprite> sprites;
    private readonly float CHARACTER_SCALE = 2.4f;

    public GameCharacter(string cName, int vitality, int armor, int strength, int magic, int mana, int maxSkill, int inventorySize){

        this.cName = cName;
        c = null;
        hp = vitality;
        this.vitality = vitality;
        this.armor = armor;
        this.strength = strength;
        this.magic = magic;
        this.mana = mana;
        this.maxMana = mana;
        skills = new Skill[maxSkill];
        skillCount = 0;
        equipment = null;
        inventory = new Item[inventorySize];

    }

    public virtual void Init(){

        equipment = gameObject.GetComponent<Equipment>();
        
    }

    public void SetSprite() {

        spriteManager = GetComponentInChildren<SpriteManager>();
        if(spriteManager == null) {
            Debug.Log("spriteManager Not found");
            return;
        }
        spriteManager.SetCharacter(this);
        moveCharacterSprite = gameObject.transform.GetChild(0);
        moveCharacterSprite.localScale = new Vector3(CHARACTER_SCALE,CHARACTER_SCALE,CHARACTER_SCALE);
    }

    public void Moved(){

        bars.position = Camera.main.WorldToScreenPoint(transform.position*0.73f);

    }

    void OnMouseDown(){

        if(c != null)
            c.CharacterClicked(this);

    }

    void OnMouseEnter(){
        
        if(c != null)
            c.CharacterHover(this);

    }

    public bool SelectSkill(int index){

        if(index < 0)
            return false;

        if(index > skillCount - 1)
            return false;

        if(skills[index].Cooldown > 0)
            return false;

        selectedSkill = index;
        return true;

    }

    public bool UseSkill(GameCharacter target){

        bool skill = target != null && skills[selectedSkill].Effect(target);
        healthBar.UpdateBar(HP, Vitality);

        Vector3 posOfTarget = target.transform.GetChild(0).position;
        if (spriteManager != null && skill) {
            skills[selectedSkill].SkillAnimation(posOfTarget, this, spriteManager);
        }

        return skill;

    }
    
    public void TakeDamage(int dmg){

        print($"{cName} is attacked with {dmg} damage and has {Armor} armor");

        if(dmg <= Armor)
            return;

        hp -= dmg - Armor;

        healthBar.UpdateBar(hp, Vitality);

        if(hp <= 0)
            c.KillCharacter(this);
        else
            DamageEffect();

    }

    async Task DamageEffect(){

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        float time = 0.5f;
        
        while(time > 0){

            sr.color = new Color(1,1-time,1-time);
            time -= Time.deltaTime;
            await Task.Yield();

        }
            
        sr.color = new Color(1,1,1);

    }

    public void AddSkill(Skill newSkill) {
        if (skillCount == skills.Length) {
            Debug.Log("Not enough slots!!");
            return;
        }

        skills[skillCount] = newSkill;
        skillCount++;
    }

    //Update the player stats (permanently).
    public void UpdateStats(int vitDelta, int strDelta, int magDelta){
        vitality += vitDelta;
        strength += strDelta;
        magic += magDelta;
    }

    public float GetEquipmentMult(int type){

        float factor = 1;

        if(equipment.head != null){
            switch(type){
                case 0:
                    factor *= equipment.head.VitalityMult;
                    break;
                case 1:
                    factor *= equipment.head.ArmorMult;
                    break;
                case 2:
                    factor *= equipment.head.StrengthMult;
                    break;
                case 3:
                    factor *= equipment.head.MagicMult;
                    break;
                case 4:
                    factor *= equipment.head.ManaMult;
                    break;
            }
        }

        if(equipment.torso != null){
            switch(type){
                case 0:
                    factor *= equipment.torso.VitalityMult;
                    break;
                case 1:
                    factor *= equipment.torso.ArmorMult;
                    break;
                case 2:
                    factor *= equipment.torso.StrengthMult;
                    break;
                case 3:
                    factor *= equipment.torso.MagicMult;
                    break;
                case 4:
                    factor *= equipment.torso.ManaMult;
                    break;
            }
        }

        if(equipment.boots != null){
            switch(type){
                case 0:
                    factor *= equipment.boots.VitalityMult;
                    break;
                case 1:
                    factor *= equipment.boots.ArmorMult;
                    break;
                case 2:
                    factor *= equipment.boots.StrengthMult;
                    break;
                case 3:
                    factor *= equipment.boots.MagicMult;
                    break;
                case 4:
                    factor *= equipment.boots.ManaMult;
                    break;
            }
        }

        if(equipment.weaponLeft != null){
            switch(type){
                case 0:
                    factor *= equipment.weaponLeft.VitalityMult;
                    break;
                case 1:
                    factor *= equipment.weaponLeft.ArmorMult;
                    break;
                case 2:
                    factor *= equipment.weaponLeft.StrengthMult;
                    break;
                case 3:
                    factor *= equipment.weaponLeft.MagicMult;
                    break;
                case 4:
                    factor *= equipment.weaponLeft.ManaMult;
                    break;
            }
        }

        if(equipment.weaponRight != null){
                switch(type){
                case 0:
                    factor *= equipment.head.VitalityMult;
                    break;
                case 1:
                    factor *= equipment.head.ArmorMult;
                    break;
                case 2:
                    factor *= equipment.head.StrengthMult;
                    break;
                case 3:
                    factor *= equipment.head.MagicMult;
                    break;
                case 4:
                    factor *= equipment.head.ManaMult;
                    break;
            }
        }

        return factor;

    }

    public int GetEquipmentSum(int type){

        int sum = 1;

        if(equipment.head != null){
            switch(type){
                case 0:
                    sum += equipment.head.VitalityAdd;
                    break;
                case 1:
                    sum += equipment.head.ArmorAdd;
                    break;
                case 2:
                    sum += equipment.head.StrengthAdd;
                    break;
                case 3:
                    sum += equipment.head.MagicAdd;
                    break;
                case 4:
                    sum += equipment.head.ManaAdd;
                    break;
            }
        }

        if(equipment.torso != null){
            switch(type){
                case 0:
                    sum += equipment.torso.VitalityAdd;
                    break;
                case 1:
                    sum += equipment.torso.ArmorAdd;
                    break;
                case 2:
                    sum += equipment.torso.StrengthAdd;
                    break;
                case 3:
                    sum += equipment.torso.MagicAdd;
                    break;
                case 4:
                    sum += equipment.torso.ManaAdd;
                    break;
            }
        }

        if(equipment.boots != null){
            switch(type){
                case 0:
                    sum += equipment.boots.VitalityAdd;
                    break;
                case 1:
                    sum += equipment.boots.ArmorAdd;
                    break;
                case 2:
                    sum += equipment.boots.StrengthAdd;
                    break;
                case 3:
                    sum += equipment.boots.MagicAdd;
                    break;
                case 4:
                    sum += equipment.boots.ManaAdd;
                    break;
            }
        }

        if(equipment.weaponLeft != null){
            switch(type){
                case 0:
                    sum += equipment.weaponLeft.VitalityAdd;
                    break;
                case 1:
                    sum += equipment.weaponLeft.ArmorAdd;
                    break;
                case 2:
                    sum += equipment.weaponLeft.StrengthAdd;
                    break;
                case 3:
                    sum += equipment.weaponLeft.MagicAdd;
                    break;
                case 4:
                    sum += equipment.weaponLeft.ManaAdd;
                    break;
            }
        }

        if(equipment.weaponRight != null){
                switch(type){
                case 0:
                    sum += equipment.head.VitalityAdd;
                    break;
                case 1:
                    sum += equipment.head.ArmorAdd;
                    break;
                case 2:
                    sum += equipment.head.StrengthAdd;
                    break;
                case 3:
                    sum += equipment.head.MagicAdd;
                    break;
                case 4:
                    sum += equipment.head.ManaAdd;
                    break;
            }
        }

        return sum;

    }

}