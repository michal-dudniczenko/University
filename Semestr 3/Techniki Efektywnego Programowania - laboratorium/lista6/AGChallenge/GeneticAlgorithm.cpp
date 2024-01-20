#include "GeneticAlgorithm.h"

#include <vector>
#include <random>


//konstruktor bez parametrow algorytmu, uzyte zostaja domyslne wartosci
GeneticAlgorithm::GeneticAlgorithm(CLFLnetEvaluator& evaluator)
	:evaluator(evaluator)
{
	this->popSize = defaultPopSize;
	this->crossProb = defaultCrossProb;
	this->mutProb = defaultMutProb;
	initializePopulation();
}

//konstruktor przyjmujacy dokladne parametry wykonania algorytmu
GeneticAlgorithm::GeneticAlgorithm(CLFLnetEvaluator& evaluator, int popSize, double crossProb, double mutProb)
	:evaluator(evaluator)
{
	this->popSize = popSize;
	this->crossProb = crossProb;
	this->mutProb = mutProb;
	initializePopulation();
}

//metoda tworzy populacje rozwiazan, kazde z nich bedzie losowo wygenerowane
void GeneticAlgorithm::initializePopulation() {
	population = new std::vector<Individual*>(popSize);
	for (int i = 0; i < this->popSize; i++) {
		population->at(i) = new Individual(evaluator);
	}
}

//destruktor, usuwa kazde z rozwiazan przechowywanych w populacji oraz sam¹ populacje
GeneticAlgorithm::~GeneticAlgorithm() {
	for (int i = 0; i < population->size(); i++) {
		delete population->at(i);
	}
	delete population;
}

//metoda generuje kolejna populacje rozwiazan
void GeneticAlgorithm::runIteration() {
	
	//przygotowanie generatorow liczb losowych
	std::random_device rd;
	std::mt19937 generator(rd());
	std::uniform_int_distribution<int> distribution(0, this->popSize-1);
	std::uniform_real_distribution<double> distribution2(0.0, 1.0);
	int random_number1;
	int random_number2;
	
	//tworze pusty vector na nastepna populacje
	std::vector<Individual*>* nextGeneration = new std::vector<Individual*>(popSize);

	//wskaznik pomocniczy na jakiej pozycji bede umieszczal kolejnych osobnikow w vectorze wynikowym
	int nextGenSlot = 0;
	
	//kazda iteracja generuje dwoch osobnikow do przyszlej populacji, dlatego popSize/2
	for (int i = 0; i < this->popSize/2; i++) {
		Individual* parent1;
		Individual* parent2;
		int first_choice;

		//losuje dwoch osobnikow
		random_number1 = distribution(generator);
		random_number2 = distribution(generator);

		//na pierwszego rodzica wybieram tego ktory ma wiekszy fitness, i zapisuje ze zostal wybrany (first_choice)
		if (compareIndividuals(population->at(random_number1), population->at(random_number2))) {
			parent1 = population->at(random_number2);
			first_choice = random_number2;
		}
		else {
			parent1 = population->at(random_number1);
			first_choice = random_number1;
		}

		//losuje kolejnych dwoch osobnikow, kandydatow na drugiego rodzica
		random_number1 = distribution(generator);
		random_number2 = distribution(generator);

		//znowu wybieram tego ktory ma wiekszy fitness, chyba ze to ten ktory jest juz pierwszym rodzicem, wtedy wybieram drugiego
		if (compareIndividuals(population->at(random_number1), population->at(random_number2))) {
			if (random_number2 != first_choice) {
				parent2 = population->at(random_number2);
			}
			else {
				parent2 = population->at(random_number1);
			}
		}
		else {
			if (random_number1 != first_choice) {
				parent2 = population->at(random_number1);
			}
			else {
				parent2 = population->at(random_number2);
			}
		}

		//losuje czy krzyzowanie zajdzie
		if (distribution2(generator) <= crossProb) {
			//jezeli tak to wykonuje krzyzowanie i otrzymane dzieci zapisuje w nowej populacji rozwiazan
			std::vector<Individual*>* children = parent1->cross(parent2);
			nextGeneration->at(nextGenSlot) = children->at(0);
			nextGeneration->at(nextGenSlot + 1) = children->at(1);
			//przesuwam miejsce wstawienia nastepnych osobnikow i usuwam wygenerowany dynamiczny vector przez metode cross
			nextGenSlot += 2;
			delete children;
		}
		else {
			//jezeli krzyzowanie nie zachodzi to do kolejnej populacji przechodza kopie rodzicow
			nextGeneration->at(nextGenSlot) = new Individual(evaluator, parent1);
			nextGeneration->at(nextGenSlot+1) = new Individual(evaluator, parent2);
			nextGenSlot += 2;
		}
	}

	//dla kazdego osobnika w nowej populacji wykonuje mutacje
	for (int i = 0; i < nextGeneration->size(); i++) {
		nextGeneration->at(i)->mutate(mutProb);
	}

	//dealokuje wszystkich osobnikow ze starej populacji oraz sama populacje
	for (int i = 0; i < population->size(); i++) {
		delete population->at(i);
	}
	delete population;

	//ustawiam nowo wygenerowana populacje jako aktualna
	population = nextGeneration;
}

//metoda po prostu uruchamia podana liczbe iteracji algorytmu
void GeneticAlgorithm::runNIterations(int n) {
	for (int i = 0; i < n; i++) {
		runIteration();
	}
}

//metoda przechodzi po wszystkich osobnikach i wyszukuje tego ktory ma najwieksze przystosowanie
Individual* GeneticAlgorithm::getBest() {
	
	//na poczatku najlepszy osobnik ustawiony jako pierwszy z populacji
	Individual* bestInd = population->at(0);
	double bestFitness = population->at(0)->getFitness();

	//przechodzimy po wszystkich pozostalych osobnikach i wyszukujemy najlepszego z nich
	for (int i = 1; i < population->size(); i++) {
		if (population->at(i)->getFitness() > bestFitness) {
			bestInd = population->at(i);
			bestFitness = population->at(i)->getFitness();
		}
	}
	//zwracamy wskaznik na osobnika o najwiekszym przystosowaniu
	return bestInd;
}

//zwraca true gdy pierwszy osobnik jest gorszy od drugiego, false w przeciwnym przypadku
bool GeneticAlgorithm::compareIndividuals(Individual* ind1, Individual* ind2) {
	return (ind1->getFitness() < ind2->getFitness());
}