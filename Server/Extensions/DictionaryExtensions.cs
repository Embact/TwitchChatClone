namespace SignalR.Extensions
{
    public static class DictionaryExtensions
    {
        public static void RenameKey<TKey,TValue>(this Dictionary<TKey,TValue> dic, TKey oldKey, TKey newKey) 
        {
            TValue dictionaryValue = dic[oldKey];
            dic.Remove(oldKey);
            dic[newKey] = dictionaryValue;
        }
    }
}
