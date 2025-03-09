namespace Domain.Bucket
{
	/// <summary>
	/// Gerencia uma coleção de buckets, permitindo inserção e busca de tuplas.
	/// </summary>
	/// <remarks>
	/// Constrói uma instância de <see cref="BucketManager"/> com a capacidade especificada.
	/// </remarks>
	/// <param name="capacity">Capacidade máxima de buckets.</param>
	public class BucketManager(int capacity)
	{
		/// <summary>
		/// A capacidade máxima de buckets que este gerenciador pode armazenar.
		/// </summary>
		public readonly int capacity = capacity;

		/// <summary>
		/// A lista de buckets armazenados no gerenciador.
		/// </summary>
		private readonly List<BucketModel> buckets = [];

		/// <summary>
		/// Gerenciador de buckets de overflow para quando a capacidade é atingida.
		/// </summary>
		private BucketManager? overflowBucketManager;

		/// <summary>
		/// Insere uma nova tupla em um bucket, criando um novo bucket caso necessário.
		/// </summary>
		/// <param name="key">A chave do bucket onde a tupla será inserida.</param>
		/// <param name="word">A palavra a ser inserida na tupla.</param>
		/// <param name="pageAddress">O endereço da página associado à palavra.</param>
		public void CreateBucket(string key, string word, int pageAddress)
		{
			BucketModel? bucket = buckets.Find(bucket => bucket.Key == key);

			if (bucket is not null)
			{
				bucket.Tuples.Add(new Tuple(word, pageAddress));
			}
			else
			{
				if (buckets.Count < capacity)
				{
					BucketModel newBucket = new(key);
					newBucket.Tuples.Add(new Tuple(word, pageAddress));
					buckets.Add(newBucket);
				}
				else
				{
					overflowBucketManager ??= new BucketManager(capacity);
					overflowBucketManager.CreateBucket(key, word, pageAddress);
				}
			}
		}

		/// <summary>
		/// Retorna a lista de buckets armazenados.
		/// </summary>
		/// <returns>Lista de <see cref="BucketModel"/>.</returns>
		public List<BucketModel> GetBuckets()
		{
			return buckets;
		}

		/// <summary>
		/// Retorna um bucket pelo índice fornecido.
		/// </summary>
		/// <param name="index">Índice do bucket na lista.</param>
		/// <returns>O bucket correspondente ao índice.</returns>
		public BucketModel GetBucketByIndex(int index)
		{
			return buckets[index];
		}

		/// <summary>
		/// Realiza uma busca por uma palavra específica em todos os buckets.
		/// </summary>
		/// <param name="target">A palavra a ser buscada.</param>
		/// <returns>O índice do bucket onde a palavra foi encontrada, ou -1 se não encontrado.</returns>
		public int TableScan(string target)
		{
			foreach (var bucket in buckets)
			{
				foreach (var tuple in bucket.Tuples)
				{
					if (tuple.Word.Equals(target, StringComparison.Ordinal))
					{
						return buckets.IndexOf(bucket);
					}
				}
			}

			return -1;
		}
	}
}
