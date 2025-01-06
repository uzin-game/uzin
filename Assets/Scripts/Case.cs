using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Case
{
    public List<Case> PotentialNeighbors;
    
    public int TotalNeighbors;

    public Case(List<Case> potentialNeighbours)
    {
        PotentialNeighbors = potentialNeighbours;
    }
    
}