#include "Individual.h"

#include <vector>
#include <random>


Individual::Individual(CLFLnetEvaluator& evaluator)
	:evaluator(evaluator)
{	
	genotype = new std::vector<int>();
	genotype->resize((size_t)evaluator.iGetNumberOfBits());
	for (int i = 0; i < genotype->size(); i++) {
		genotype->at(i) = lRand(evaluator.iGetNumberOfValues(i));
	}
	calcFitness();
}

Individual::Individual(CLFLnetEvaluator& evaluator, std::vector<int>* genotype) 
	:evaluator(evaluator)
{
	this->genotype = genotype;
	calcFitness();
}

Individual::Individual(CLFLnetEvaluator& evaluator, Individual* other)
	:evaluator(evaluator)
{
	this->genotype = new std::vector<int>(other->getGenotype()->size());
	for (int i = 0; i < this->genotype->size(); i++) {
		this->genotype->at(i) = other->getGenotype()->at(i);
	}
}

Individual::~Individual() {
	delete genotype;
}

double Individual::getFitness() {
	return fitness;
}

std::vector<int>* Individual::getGenotype() {
	return genotype;
}

void Individual::calcFitness() {
	fitness = evaluator.dEvaluate(genotype);
}

void Individual::mutate(double mutProb) {
	std::random_device rd;
	std::mt19937 generator(rd());
	std::uniform_real_distribution<double> distribution(0.0, 1.0);
	double random_number;
	
	for (int i = 0; i < genotype->size(); i++) {
		if ((random_number = distribution(generator)) <= mutProb) {
			genotype->at(i) = lRand(evaluator.iGetNumberOfValues(i));
		}
	}
	calcFitness();
}

std::vector<Individual*>* Individual::cross(Individual* other) {
	std::random_device rd;
	std::mt19937 generator(rd());
	std::uniform_int_distribution<int> distribution(1, this->genotype->size()-1);
	int crossPoint = distribution(generator);

	std::vector<int>* genotype1 = new std::vector<int>(this->genotype->size());
	std::vector<int>* genotype2 = new std::vector<int>(this->genotype->size());

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

	std::vector<Individual*>* result = new std::vector<Individual*>(2);
	result->at(0) = new Individual(evaluator, genotype1);
	result->at(1) = new Individual(evaluator, genotype2);

	return result;
}