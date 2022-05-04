using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using CSharp_ECS;

namespace Game.Ecosim;

struct Person : IComponent
{
    public int Id { get; set; }

    public string Name;
    public string FullName;
    public string Title;

    public int JobId;
    public int CurrentActivityId;
    public int PlannedActivityId;

    public Personality Personality;

    public List<int> Likes;
}

struct Personality
{
    public float Risky;
    public float Greedy;
    public float Generous;
    public float Stubborness;
    public float Happiness;
}
