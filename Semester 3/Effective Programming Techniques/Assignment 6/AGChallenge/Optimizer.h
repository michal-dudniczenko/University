//dodalem wskaznik na instancje algorytmu genetycznego, destruktor, zmodyfikowalem metodê vInitialize oraz vRunIteration
//aby korzystac z mojej implementacji algorytu genetycznego zamiast losowo generowac rozwiazania

#pragma once

#include "Evaluator.h"
#include "GeneticAlgorithm.h"

#include <random>
#include <vector>

using namespace std;

class COptimizer
{
public:
	COptimizer(CLFLnetEvaluator &cEvaluator);
	~COptimizer();

	void vInitialize();
	void vRunIteration();
	vector<int> *pvGetCurrentBest() { return &v_current_best; }

private:
	CLFLnetEvaluator &c_evaluator;
	GeneticAlgorithm* genAlg;
	double d_current_best_fitness;
	vector<int> v_current_best;
	mt19937 c_rand_engine;

	void v_fill_randomly(vector<int>& vSolution);
};//class COptimizer