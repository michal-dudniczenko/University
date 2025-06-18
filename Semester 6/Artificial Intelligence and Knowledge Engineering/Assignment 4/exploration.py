import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import missingno as msno

# Wczytanie danych
df = pd.read_csv('dane.csv')

# Ustawienia wykresów
sns.set_theme(style="whitegrid")
plt.rcParams['figure.figsize'] = (12, 6)

# --- 1. Podstawowe informacje o zbiorze ---
print("▶️ Podstawowe informacje o zbiorze:")
print(df.info())
print("\n▶️ Podsumowanie statystyczne (pierwsze 5 kolumn):")
print(df.describe(include='all').T.head())

# --- 2. Liczba braków danych ---
print("\n▶️ Liczba brakujących wartości:")
print(df.isnull().sum())

# --- 3. Wizualizacja braków danych ---
print("\n▶️ Wizualizacja braków danych:")
msno.matrix(df)
plt.title("Braki danych (Missing Data Matrix)")
plt.show()

# --- 4. Histogramy cech numerycznych ---
print("\n▶️ Histogramy cech numerycznych:")
num_features = df.columns.drop('CLASS')
df[num_features].hist(bins=20, figsize=(16, 14), layout=(6, 4))
plt.suptitle("Histogramy cech", y=1.02)
plt.tight_layout()
plt.show()

# --- 5. Korelacja cech ---
print("\n▶️ Macierz korelacji cech:")
corr = df[num_features].corr()
sns.heatmap(corr, annot=False, cmap='coolwarm', fmt=".2f")
plt.title("Macierz korelacji cech diagnostycznych")
plt.show()

# --- 6. Rozkład klas (zmienna CLASS) ---
print("\n▶️ Rozkład klas:")
class_counts = df['CLASS'].value_counts().sort_index()
print(class_counts)

sns.countplot(data=df, x='CLASS', hue='CLASS', palette='tab10', legend=False)
plt.title("Rozkład klas (CLASS)")
plt.xlabel("Klasa morfologiczna")
plt.ylabel("Liczba przypadków")
plt.show()

# --- 7. Statystyki opisowe dla każdej cechy ---
print("\n▶️ Statystyki opisowe cech:")
stats = df[num_features].agg(['count', 'min', 'max', 'mean', 'median', 'std']).T
print(stats.sort_index())