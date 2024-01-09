#include "Optimizer.h"

#include <cfloat>
#include <iostream>
#include <windows.h>

using namespace std;

COptimizer::COptimizer(CLFLnetEvaluator &cEvaluator)
	: c_evaluator(cEvaluator), genAlg(nullptr)
{
	random_device c_seed_generator;
	c_rand_engine.seed(c_seed_generator());

	d_current_best_fitness = 0;
}//COptimizer::COptimizer(CEvaluator &cEvaluator)

void COptimizer::vInitialize()
{
	d_current_best_fitness = -DBL_MAX;
	v_current_best.clear();
	
	genAlg = new GeneticAlgorithm(c_evaluator);
}//void COptimizer::vInitialize()

void COptimizer::vRunIteration()
{
	genAlg->runIteration();
	
	double d_candidate_fitness = genAlg->getBest()->getFitness();

	if (d_candidate_fitness > d_current_best_fitness)
	{
		v_current_best = *(genAlg->getBest()->getGenotype());
		d_current_best_fitness = d_candidate_fitness;

		cout << d_current_best_fitness << endl;
	}//if (d_candidate_fitness > d_current_best_fitness)
}//void COptimizer::vRunIteration()



COptimizer::~COptimizer() {
	delete this->genAlg;
}

void COptimizer::v_fill_randomly(vector<int> &vSolution)
{
	vSolution.resize((size_t)c_evaluator.iGetNumberOfBits());

	for (int ii = 0; ii < vSolution.size(); ii++)
	{
		vSolution.at(ii) = lRand(c_evaluator.iGetNumberOfValues(ii));
	}//for (size_t i = 0; i < vSolution.size(); i++)
}//void COptimizer::v_fill_randomly(const vector<int> &vSolution)
