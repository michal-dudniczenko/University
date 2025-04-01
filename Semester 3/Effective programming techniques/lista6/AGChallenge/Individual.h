//klasa reprezentujaca pojedyncze rozwiazanie, osobnika populacji przechowywanej w algorytmie genetycznym
//osobnik posiada genotyp kodujacy rozwiazanie, przechowuje swoja ocene (przystosowanie) - fitness
//posiada metode calcFitness() sluzaca do oceny przechowywanego rozwiazania
//3 konstruktory, destruktor, gettery, metode mutuj¹c¹ przyjmujaca prawdopodobienstwo na mutacje
//oraz metode cross ktora krzyzuje obecnego osobnika z innym, podanym jako argument i zwraca dwojke dzieci w vectorze

#pragma once

#include "Evaluator.h"

#include <vector>


class Individual {
private:
	std::vector<int>* genotype;
	CLFLnetEvaluator& evaluator;
	double fitness;

	void calcFitness();

public:
	Individual(CLFLnetEvaluator& evaluator);
	Individual(CLFLnetEvaluator& evaluator, std::vector<int>* genotype);
	Individual(CLFLnetEvaluator& evaluator, Individual* other);
	~Individual();

	double getFitness();
	std::vector<int>* getGenotype();

	void mutate(double mutProb);
	std::vector<Individual*>* cross(Individual* other);
};