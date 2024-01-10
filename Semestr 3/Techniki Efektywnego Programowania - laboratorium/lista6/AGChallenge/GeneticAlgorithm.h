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