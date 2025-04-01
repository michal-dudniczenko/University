#include "Individual.h"

#include <vector>
#include <random>


//inicjalizacja podanym obiektem evaluatora z udostepnionego frameworku
Individual::Individual(CLFLnetEvaluator& evaluator)
	:evaluator(evaluator)
{	
	//generujemy losowego osobnika (o losowym genotypie - rozwiazaniu) korzystajac z evaluatora
	genotype = new std::vector<int>();
	genotype->resize((size_t)evaluator.iGetNumberOfBits());
	for (int i = 0; i < genotype->size(); i++) {
		genotype->at(i) = lRand(evaluator.iGetNumberOfValues(i));
	}
	//i obliczamy jego przystosowanie
	calcFitness();
}

//inicjalizacja podanym obiektem evaluatora z udostepnionego frameworku
Individual::Individual(CLFLnetEvaluator& evaluator, std::vector<int>* genotype) 
	:evaluator(evaluator)
{
	//przepisujemy wskaznik na genotyp
	this->genotype = genotype;
	calcFitness();
}

//inicjalizacja podanym obiektem evaluatora z udostepnionego frameworku
Individual::Individual(CLFLnetEvaluator& evaluator, Individual* other)
	:evaluator(evaluator)
{	
	//kopiowanie genotypu od podanego osobnika
	this->genotype = new std::vector<int>(other->getGenotype()->size());
	for (int i = 0; i < this->genotype->size(); i++) {
		this->genotype->at(i) = other->getGenotype()->at(i);
	}
}

//destruktor, usuwanie dynamicznie zaalokowanego genotypu
Individual::~Individual() {
	delete genotype;
}

//zwraca przechowywane przystosowanie osobnika
double Individual::getFitness() {
	return fitness;
}

//zwraca wskaznik na przechodywany genotyp osobnika
std::vector<int>* Individual::getGenotype() {
	return genotype;
}

//oblicz fitness osobnika korzystajac z metody evaluatora pochodzacej z udostepnionego frameworka
void Individual::calcFitness() {
	fitness = evaluator.dEvaluate(genotype);
}

//mutacja osobnika
void Individual::mutate(double mutProb) {
	
	//przygotowanie generatora liczb losowych
	std::random_device rd;
	std::mt19937 generator(rd());
	std::uniform_real_distribution<double> distribution(0.0, 1.0);
	double random_number;
	
	//przejscie po wszystkich genach w genotypie osobnika
	for (int i = 0; i < genotype->size(); i++) {
		//dla kazdego genu losujemy czy mutacja zajdzie czy nie
		if ((random_number = distribution(generator)) <= mutProb) {
			//jezeli mutacja zachodzi to gen mutuje i zostaje zmieniony na losowa wartosc
			genotype->at(i) = lRand(evaluator.iGetNumberOfValues(i));
		}
	}
	//po przeprowadzeniu mutacji oblicz i zaktualizuj fitness osobnika
	calcFitness();
}

//funkcja do krzyzowania osobnikow, krzyzuje osobnika z osobnikiem podanym w wywolaniu, 
//zwraca dwojke dzieci w formie vectora z dwoma wskaznikami
std::vector<Individual*>* Individual::cross(Individual* other) {
	
	//przygotowanie generatora liczb losowych
	std::random_device rd;
	std::mt19937 generator(rd());
	std::uniform_int_distribution<int> distribution(1, this->genotype->size()-1);

	//losujemy punkt przeciecia genotypow
	int crossPoint = distribution(generator);

	//tworze puste genotypy dzieci
	std::vector<int>* genotype1 = new std::vector<int>(this->genotype->size());
	std::vector<int>* genotype2 = new std::vector<int>(this->genotype->size());

	//uzupelniam genotypy dzieci zgodnie z zasada: dziecko1 = czesc rodzica1 + czesc rodzica2
	//dziecko2 = czesc rodzica2 + czesc rodzica1
	int index = 0;
	for (int i = 0; i < crossPoint; i++) {
		genotype1->at(index) = this->genotype->at(i);
		genotype2->at(index) = other->genotype->at(i);
		index++;
	}
	for (int i = crossPoint; i < genotype1->size(); i++) {
		genotype1->at(index) = other->genotype->at(i);
		genotype2->at(index) = this->genotype->at(i);
		index++;
	}

	//tworze vector na zwracane dzieci i umieszczam w nim nowo tworzone dzieci, nastepnie zwracam vector
	std::vector<Individual*>* result = new std::vector<Individual*>(2);
	result->at(0) = new Individual(evaluator, genotype1);
	result->at(1) = new Individual(evaluator, genotype2);

	return result;
}