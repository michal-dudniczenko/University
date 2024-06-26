import wikipedia
import warnings

warnings.filterwarnings("ignore")

article_name = input("Podaj nazwę artykułu na Wikipedii: ")

try:
    print("\n" + wikipedia.summary(article_name, sentences=3))
    print("\nAdres URL wyszukanego artykułu:", wikipedia.page(article_name).url, "\n")
except wikipedia.exceptions.DisambiguationError as e:
    print("\n" + wikipedia.summary(e.options[0], sentences=3))
    print("\nAdres URL wyszukanego artykułu:", wikipedia.page(e.options[0]).url, "\n")
except wikipedia.exceptions.PageError:
    print("Nie znaleziono strony.")


def wiki_get_summary_url():
    article_name = input("Podaj nazwę artykułu na Wikipedii: ")

    try:
        return wikipedia.summary(article_name, sentences=3), wikipedia.page(article_name).url
    except wikipedia.exceptions.DisambiguationError as e:
        return wikipedia.summary(e.options[0], sentences=3), wikipedia.page(article_name).url
    except wikipedia.exceptions.PageError:
        return None, None
