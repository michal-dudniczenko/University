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