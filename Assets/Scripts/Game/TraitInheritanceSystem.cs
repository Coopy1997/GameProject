using System.Collections.Generic;
using UnityEngine;

public static class TraitInheritanceSystem
{
    public static List<Trait> InheritTraits(Fish a, Fish b)
    {
        List<Trait> inherited = new List<Trait>();

        TryAddParentTraits(a, inherited);
        TryAddParentTraits(b, inherited);

        List<Trait> finalList = new List<Trait>();
        for (int i = 0; i < inherited.Count; i++)
        {
            Trait t = inherited[i];
            if (t == null) continue;
            finalList.Add(t);
        }

        float r = Random.value;
        if (r <= 0.001f)
        {
            Trait legendary = TraitDatabase.GetLegendaryTrait();
            if (legendary != null)
            {
                finalList.Add(legendary);
            }
        }

        while (finalList.Count > 2)
        {
            int index = Random.Range(0, finalList.Count);
            finalList.RemoveAt(index);
        }

        if (finalList.Count < 1)
        {
            Trait randomTrait = TraitDatabase.allTraits[Random.Range(0, TraitDatabase.allTraits.Count)];
            finalList.Add(randomTrait);
        }
        else if (finalList.Count < 2 && Random.value < 0.1f)
        {
            Trait extra = TraitDatabase.allTraits[Random.Range(0, TraitDatabase.allTraits.Count)];
            finalList.Add(extra);
        }

        return finalList;
    }

    static void TryAddParentTraits(Fish parent, List<Trait> list)
    {
        if (parent == null || parent.traits == null) return;

        for (int i = 0; i < parent.traits.Count; i++)
        {
            Trait t = parent.traits[i];
            if (t == null) continue;

            if (Random.value < 0.5f)
            {
                list.Add(t);
            }
        }
    }
}
