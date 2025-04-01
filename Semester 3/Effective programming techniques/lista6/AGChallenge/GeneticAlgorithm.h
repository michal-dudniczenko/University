//klasa sluzy do obslugi algorytmu genetycznego, przechowuje populacje osobnikow (rozwiazan) population,
//parametry wykonania algorytmu: rozmiar populacji, prawdopodobienstwo krzyzowania, prawdopodobienstwo mutacji
//pierwsza populacja jest generowana losowo przez metode initializePopulation(), ktora korzysta
//z udostepnionego frameworku (za to odpowiada evaluator)
//statyczna metoda compareIndividuals, ktora sluzy do okreslenia ktorego osobnika przystosowanie jest wieksze
//2 konstruktory, jeden z samym evaluatorem (wtedy algorytm uruchomi sie z domyslnymi parametrami)
//drugi z konkretnymi parametrami wykonania, destruktor, dwie metody do uruchamiania kolejnych iteracji algorytmu,
//czyli generowania kolejnych populacji osobnikow, do tego metoda getBest() ktora wyszukuje osobnika o najlepszym przystosowaniu 

#pragma once

#include "Evaluator.h"
#include "Individual.h"

#include <vector>


const int defaultPopSize = 100;
const double defaultCrossProb = 0.8;
const double defaultMutProb = 0.001;


class GeneticAlgorithm {
private:
	std::vector<Individual*>* population;
	CLFLnetEvaluator& evaluator;
	int popSize;
	double crossProb;
	double mutProb;

	void initializePopulation();
	static bool compareIndividuals(Individual* ind1, Individual* ind2);

public:
	GeneticAlgorithm(CLFLnetEvaluator& evaluator);
	GeneticAlgorithm(CLFLnetEvaluator& evaluator, int popSize, double crossProb, double mutProb);
	~GeneticAlgorithm();

	void runIteration();
	void runNIterations(int n);
	Individual* getBest();
};