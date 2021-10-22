using System.Collections;
using System.Text;
using UnityEngine;

namespace MobileConsole
{
	public class TestLog : MonoBehaviour
	{
		void Awake()
		{
			LogConsole.OnVisibilityChanged += OnVisibilityChanged;
		}

		void OnVisibilityChanged(bool isActive)
		{
			Debug.Log("Active: " + isActive);
		}

		public void LogInfo(int numLog)
		{
			for (int i = 0; i < numLog; i++)
			{
				Debug.Log("[Player] Log info");
			}

		}

		public void LogWarning(int numLog)
		{
			for (int i = 0; i < numLog; i++)
			{
				Debug.LogWarning("[Physics] Log Warning");
			}
		}

		public void LogError(int numLog)
		{
			for (int i = 0; i < numLog; i++)
			{
				Debug.LogError("[Renderer,Material] Log error");
			}
		}

		public void LongLog()
		{
			Debug.Log(@"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam lobortis fermentum mauris at iaculis. Etiam accumsan diam quis est gravida gravida. Proin in posuere leo. Integer lacinia mollis quam nec dapibus. Vestibulum venenatis sagittis fringilla. Nulla at convallis purus. Praesent porta ultricies volutpat. Donec dolor sem, sollicitudin sed blandit ac, tincidunt at nulla. Aenean iaculis ligula vel maximus mattis. Vestibulum lorem quam, pharetra scelerisque tincidunt sed, lacinia vel nulla.

Morbi mattis, magna at vehicula convallis, sapien dolor malesuada elit, in convallis ex libero vitae nulla. Aenean finibus rutrum tortor, nec tristique leo suscipit ut. Phasellus varius velit eu est pharetra, vel placerat felis rutrum. Donec gravida, augue vitae cursus volutpat, mi neque condimentum dolor, quis tincidunt nibh metus eget turpis. Vestibulum elit leo, tincidunt et turpis vel, accumsan facilisis diam. Aenean luctus feugiat tellus eget venenatis. Duis non molestie erat. Vivamus luctus, tortor quis facilisis fringilla, augue velit porttitor eros, et auctor lacus sem ac tortor. Integer a ornare libero. Curabitur cursus interdum mattis.

Mauris eget lacus nec nunc bibendum consequat. Fusce auctor eros non mi posuere volutpat. Etiam aliquam, urna sit amet efficitur tempus, ipsum nunc convallis metus, et gravida ipsum ante et quam. Duis eu mi sed magna vestibulum cursus. Maecenas eget urna justo. Praesent sit amet velit augue. Vestibulum nisi ipsum, tincidunt sit amet tincidunt ut, hendrerit in mauris. Suspendisse aliquet, tortor ut imperdiet gravida, augue ex tempor lectus, in aliquet ex felis sed erat. Etiam lobortis mi eu volutpat fringilla. Sed non sagittis diam. Maecenas laoreet mi ut suscipit auctor. Vestibulum consequat iaculis quam nec imperdiet. Cras ut aliquet magna, sit amet rutrum quam. Vivamus laoreet, sapien ac condimentum ullamcorper, velit nisl aliquam leo, in euismod lacus tellus vel ante. Curabitur imperdiet magna eu orci maximus, posuere egestas nisl tempor. Sed nec neque sed mauris mollis eleifend.

Aliquam nibh sem, egestas ac sollicitudin eu, pharetra vitae elit. Aliquam venenatis magna in ipsum tincidunt, id eleifend arcu porttitor. Nullam porttitor vel purus sit amet posuere. Phasellus tempus nibh quis enim condimentum pretium. Proin ultricies vehicula ex, eu sagittis tellus pellentesque ac. Aliquam luctus libero nec eleifend tincidunt. Integer condimentum dignissim lorem, at posuere ex elementum vitae. Aenean ut dictum arcu. Suspendisse suscipit et nisl et feugiat. Donec imperdiet elementum pharetra. Aliquam varius diam mi, eu rutrum turpis feugiat sed.

Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Pellentesque quis mi vitae urna convallis feugiat ac at felis. Sed scelerisque facilisis elit non faucibus. Integer ullamcorper augue a quam dictum pretium. Phasellus consequat nisi quis augue fringilla pretium. Donec aliquet nisi eget tortor venenatis congue. Proin magna turpis, viverra non elementum eget, pellentesque sed libero. Maecenas quis volutpat sapien. Fusce ac faucibus lacus.");
		}

		public void ThrowException()
		{
			throw new System.Exception("This is definitely an exception, dont you see?");
		}

		public void DivideByZero()
		{
			int a = 1;
			int b = 0;
			int c = a / b;
			Debug.Log(c);
		}

		Coroutine _genLogCoroutine;
		public void GenLogAfter1Sec()
		{
			if (_genLogCoroutine == null)
			{
				_genLogCoroutine = StartCoroutine(GenLog());
			}
		}

		public void StopGenLog()
		{
			if (_genLogCoroutine != null)
			{
				StopCoroutine(_genLogCoroutine);
				_genLogCoroutine = null;
			}
		}

		IEnumerator GenLog()
		{
			System.Random rnd = new System.Random();
			while (true)
			{
				Debug.LogWarning("[" + GenerateString(7, rnd) + "] " + GenerateString(100, rnd));
				yield return new WaitForSeconds(1);
			}
		}

		public static string GenerateString(int length, System.Random random)
		{
			string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			StringBuilder result = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				result.Append(characters[random.Next(characters.Length)]);
			}
			return result.ToString();
		}

		public void ShowLog()
		{
			LogConsole.ToggleShow();
		}
	}
}
