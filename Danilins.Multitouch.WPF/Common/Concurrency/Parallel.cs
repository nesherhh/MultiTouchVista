using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Danilins.Multitouch.Common.Concurrency
{
	public static class Parallel
	{
		public static void For(int size, Action<int> body)
		{
			Context context = new Context(0, size, body);
			GenerateTasks(context);
			Iterate(context);
		}

		public static void For(int from, int size, Action<int> body)
		{
			Context context = new Context(from, size, body);
			GenerateTasks(context);
			Iterate(context);
		}

		private static void GenerateTasks(Context context)
		{
			int chunkSize = context.ChunkSize;
			for (int i = context.From; i < context.Size / chunkSize; i++)
			{
				int from = i * chunkSize;
				int to = (i + 1) * chunkSize;
				context.Tasks.Enqueue(new Task(from, to));
			}
		}

		private static void Iterate(Context context)
		{
			int count = context.ProcessorCount;
			for (int i = 0; i < count; i++)
				ThreadPool.QueueUserWorkItem(Worker, context);

			context.WaitForCompletion();
		}

		private static void Worker(object state)
		{
			Context context = (Context)state;
			Task task;
			do
			{
				task = null;
				Queue<Task> tasks = context.Tasks;
				lock (tasks)
				{
					if (tasks.Count > 0)
						task = tasks.Dequeue();
				}
				if (task != null)
				{
					int to = task.To;
					int from = task.From;
					for (int i = from; i < to; i++)
						context.Body(i);
				}
			} while (task != null);
			context.DecrementCount();
		}

		private class Context
		{
			private readonly int from;
			private readonly int size;
			private readonly Action<int> body;
			private Queue<Task> tasks;
			private AutoResetEvent completedEvent;
			private int counter;

			public Context(int from, int size, Action<int> body)
			{
				this.from = from;
				this.size = size;
				this.body = body;
				tasks = new Queue<Task>();
				counter = ProcessorCount;
				completedEvent = new AutoResetEvent(false);
			}

			public int ProcessorCount
			{
				get { return Environment.ProcessorCount * 2; }
			}

			public int ChunkSize
			{
				get { return 1; } //Size / ProcessorCount; }
			}

			public int From
			{
				get { return from; }
			}

			public int Size
			{
				get { return size; }
			}

			public Action<int> Body
			{
				get { return body; }
			}

			public Queue<Task> Tasks
			{
				get { return tasks; }
			}

			public void WaitForCompletion()
			{
				completedEvent.WaitOne();
			}

			public void DecrementCount()
			{
				if (Interlocked.Decrement(ref counter) == 0)
					completedEvent.Set();
			}
		}

		[DebuggerDisplay("{From} - {To}")]
		private class Task
		{
			private readonly int from;
			private readonly int to;

			public Task(int from, int to)
			{
				this.from = from;
				this.to = to;
			}

			public int From
			{
				get { return from; }
			}

			public int To
			{
				get { return to; }
			}
		}
	}
}