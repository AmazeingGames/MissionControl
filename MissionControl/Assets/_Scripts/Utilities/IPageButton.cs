using UnityEngine;

public interface IPageButton 
{
    public void Initialize<T>(T data) where T : IPageData;
}

public interface IPageData { }

