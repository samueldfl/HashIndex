namespace Domain.Pkg;
public static class Utils
{
    /// <summary>
    /// Método para calcular o hash de uma string.
    /// </summary>
    /// <param name="input">A string de entrada.</param>
    /// <param name="numBuckets">O número de buckets.</param>
    /// <returns>O hash calculado.</returns>
    public static int ComputeHash(string key, int numBuckets)
    {
        int hash = 0;
        foreach (char c in key)
        {
            hash = (hash * 31 + c) % numBuckets;
        }
        return Math.Abs(hash);
    }
}