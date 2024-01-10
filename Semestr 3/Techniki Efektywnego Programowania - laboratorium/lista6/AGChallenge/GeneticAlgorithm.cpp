#include "GeneticAlgorithm.h"

#include <vector>
#include <random>


GeneticAlgorithm::GeneticAlgorithm(CLFLnetEvaluator& evaluator)
	:evaluator(evaluator)
{
	this->popSize = defaultPopSize;
	this->crossProb = defaultCrossProb;
	this->mutProb = defaultMutProb;
	initializePopulation();
}

GeneticAlgorithm::GeneticAlgorithm(CLFLnetEvaluator& evaluator, int popSize, double crossProb, double mutProb)
	:evaluator(evaluator)
{
	this->popSize = popSize;
	this->crossProb = crossProb;
	this->mutProb = mutProb;
	initializePopulation();
}

void GeneticAlgorithm::initializePopulation() {
	population = new std::vector<Individual*>(popSize);
	for (int i = 0; i < this->popSize; i++) {
		population->at(i) = new Individual(evaluator);
	}
}

GeneticAlgorithm::~GeneticAlgorithm() {
	for (int i = 0; i < population->size(); i++) {
		delete population->at(i);
	}
	delete population;
}

void GeneticAlgorithm::runIteration() {
	std::random_device rd;
	std::mt19937 generator(rd());
	std::uniform_int_distribution<int> distribution(0, this->popSize-1);
	std::uniform_real_distribution<double> distribution2(0.0, 1.0);
	int random_number1;
	int random_number2;
	
	std::vector<Individual*>* nextGeneration = new std::vector<Individual*>(popSize);

	int nextGenSlot = 0;
	
	for (int i = 0; i < this->popSize/2; i++) {
		Individual* parent1;
		Individual* parent2;
		int first_choice;

		random_number1 = distribution(generator);
		random_number2 = distribution(generator);

		if (compareIndividuals(population->at(random_number1), population->at(random_number2))) {
			parent1 = population->at(random_number2);
			first_choice = random_number2;
		}
		else {
			parent1 = population->at(random_number1);
			first_choice = random_number1;
		}

		random_number1 = distribution(generator);
		random_number2 = distribution(generator);

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

		if (distribution2(generator) <= crossProb) {
			std::vector<Individual*>* children = parent1->cross(parent2);
			nextGeneration->at(nextGenSlot) = children->at(0);
			nextGeneration->at(nextGenSlot + 1) = children->at(1);
			nextGenSlot += 2;
			delete children;
		}
		else {
			nextGeneration->at(nextGenSlot) = new Individual(evaluator, parent1);
			nextGeneration->at(nextGenSlot+1) = new Individual(evaluator, parent2);
			nextGenSlot += 2;
		}
	}

	for (int i = 0; i < nextGeneration->size(); i++) {
		nextGeneration->at(i)->mutate(mutProb);
	}

	for (int i = 0; i < population->size(); i++) {
		delete population->at(i);
	}
	delete population;

	population = nextGeneration;
}

void GeneticAlgorithm::runNIterations(int n) {
	for (int i = 0; i < n; i++) {
		runIteration();
	}
}

Individual* GeneticAlgorithm::getBest() {
	Individual* bestInd = population->at(0);
	double bestFitness = population->at(0)->getFitness();

	for (int i = 1; i < population->size(); i++) {
		if (population->at(i)->getFitness() > bestFitness) {
			bestInd = population->at(i);
			bestFitness = population->at(i)->getFitness();
		}
	}
	return bestInd;
}

//zwraca true gdy pierwszy jest gorszy od drugiego
bool GeneticAlgorithm::compareIndividuals(Individual* ind1, Individual* ind2) {
	return (ind1->getFitness() < ind2->getFitness());
}