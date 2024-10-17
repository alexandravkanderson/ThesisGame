using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShell : Characters
{
    // Player (Ghost)
    private CharacterGhost characterGhost;
    
    // Attacking
    public float ShellHP
    {
        get
        {
            return base.HP;
        }
        set
        {
            base.HP = value;
            
            if (base.HP <=0 )
            {
                // If the shell's HP is 0, end the game,
                // player wins
                GameManager.instance.isPlayerWin = true;
                GameManager.instance.GameEnding();
            }
            else
            {
                Debug.Log("Shell HP: " + base.HP);
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        characterGhost = GameManager.instance.characterGhost;
    }
    
    public void ShellAttack()
    {
        // Deal damage every "attack speed" second
        InvokeRepeating(nameof(ShellDealDamage), base.SP, base.SP);
    }

    private void ShellDealDamage()
    {
        // If the game isn't ended, dell "AD" amount of damage
        if (!GameManager.instance.isGameEnd)
        {
            characterGhost.playerHP -= AD;
        }
    }
}
