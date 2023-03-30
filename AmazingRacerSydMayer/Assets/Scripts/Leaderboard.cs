using System;

public class Leaderboard
{
    public Guid Id { get; set; }
    public string UserName { get;set;}
    
    public DateTime GameTime { get; set; }
    
    public int Score { get; set; }

    public override string ToString()
    {
        return $"{UserName}: {Score} ({GameTime})";
    }

}