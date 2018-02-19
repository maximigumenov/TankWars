using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class Damage
{
    public static int SetDamage(float defense, int hitDemage) {
        return (int)(hitDemage * defense);
    }
}

