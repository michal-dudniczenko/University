import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.impute import SimpleImputer
from sklearn.naive_bayes import GaussianNB
from sklearn.preprocessing import StandardScaler, MinMaxScaler
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, confusion_matrix, f1_score, precision_score, recall_score
from sklearn.svm import SVC
from sklearn.tree import DecisionTreeClassifier

# Wczytanie danych
df = pd.read_csv("dane.csv")  # zastąp nazwą swojego pliku

# Rozdziel cechy i etykiety
X = df.drop(columns=['CLASS'])
y = df['CLASS']

# Uzupełnianie braków (imputacja średnią)
imputer = SimpleImputer(strategy='mean')
X_imputed = imputer.fit_transform(X)

# Podział na dane treningowe i testowe
X_train, X_test, y_train, y_test = train_test_split(X_imputed, y, test_size=0.3, random_state=42, stratify=y)

# Normalizacja (Min-Max Scaling)
scaler_norm = MinMaxScaler()
X_train_norm = scaler_norm.fit_transform(X_train)
X_test_norm = scaler_norm.transform(X_test)

# Standaryzacja (Z-score)
scaler_std = StandardScaler()
X_train_std = scaler_std.fit_transform(X_train)
X_test_std = scaler_std.transform(X_test)


# Funkcja do obliczania metryk
def evaluate_model(y_true, y_pred):
    cm = confusion_matrix(y_true, y_pred)
    acc = accuracy_score(y_true, y_pred)
    tpr = recall_score(y_true, y_pred, average='macro', zero_division=0)
    ppv = precision_score(y_true, y_pred, average='macro', zero_division=0)
    f1 = f1_score(y_true, y_pred, average='macro', zero_division=0)
    return cm, acc, tpr, ppv, f1

# Konfiguracje klasyfikatora Bayesa
bayes_configs = [
    GaussianNB(var_smoothing=1e-9),
    GaussianNB(var_smoothing=1e-8),
    GaussianNB(var_smoothing=1e-7)
]

# Konfiguracje drzewa decyzyjnego
tree_configs = [
    DecisionTreeClassifier(max_depth=5, min_samples_split=4, criterion='gini', random_state=42),
    DecisionTreeClassifier(max_depth=10, min_samples_split=2, criterion='entropy', random_state=42),
    DecisionTreeClassifier(max_depth=None, min_samples_split=10, criterion='gini', random_state=42)
]

# Konfiguracja lasu losowego
rf_configs = [
    RandomForestClassifier(n_estimators=100, max_depth=5, random_state=42),
    RandomForestClassifier(n_estimators=200, max_depth=10, random_state=42),
    RandomForestClassifier(n_estimators=300, max_depth=None, random_state=42)
]

# Konfiguracja SVC
svm_configs = [
    SVC(C=1.0, kernel='linear', random_state=42),
    SVC(C=1.0, kernel='rbf', gamma='scale', random_state=42),
    SVC(C=10.0, kernel='rbf', gamma='auto', random_state=42)
]

# Zestawy danych do testowania
datasets = {
    "raw": (X_train, X_test),
    "normalized": (X_train_norm, X_test_norm),
    "standardized": (X_train_std, X_test_std)
}

# Testowanie klasyfikatorów
for name, (Xtr, Xte) in datasets.items():
    print(f"\n=== Dataset: {name.upper()} ===")

    print("\n-- GaussianNB --")
    for i, model in enumerate(bayes_configs):
        model.fit(Xtr, y_train)
        y_pred = model.predict(Xte)
        cm, acc, tpr, ppv, f1 = evaluate_model(y_test, y_pred)
        print(f"Config {i+1}: var_smoothing={model.var_smoothing:.0e}")
        print(f"ACC={acc:.3f}, TPR={tpr:.3f}, PPV={ppv:.3f}, F1={f1:.3f}")

    print("\n-- Decision Tree --")
    for i, model in enumerate(tree_configs):
        model.fit(Xtr, y_train)
        y_pred = model.predict(Xte)
        cm, acc, tpr, ppv, f1 = evaluate_model(y_test, y_pred)
        print(f"Config {i+1}: max_depth={model.max_depth}, min_samples_split={model.min_samples_split}, criterion={model.criterion}")
        print(f"ACC={acc:.3f}, TPR={tpr:.3f}, PPV={ppv:.3f}, F1={f1:.3f}")
    
    print("\n-- Random Forest --")
    for i, model in enumerate(rf_configs):
        model.fit(Xtr, y_train)
        y_pred = model.predict(Xte)
        cm, acc, tpr, ppv, f1 = evaluate_model(y_test, y_pred)
        print(f"Config {i+1}: n_estimators={model.n_estimators}, max_depth={model.max_depth}")
        print(f"ACC={acc:.3f}, TPR={tpr:.3f}, PPV={ppv:.3f}, F1={f1:.3f}")

    print("\n-- SVM --")
    for i, model in enumerate(svm_configs):
        model.fit(Xtr, y_train)
        y_pred = model.predict(Xte)
        cm, acc, tpr, ppv, f1 = evaluate_model(y_test, y_pred)
        print(f"Config {i+1}: kernel={model.kernel}, C={model.C}, gamma={model.gamma}")
        print(f"ACC={acc:.3f}, TPR={tpr:.3f}, PPV={ppv:.3f}, F1={f1:.3f}")
