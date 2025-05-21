package com.example.befit.trainingprograms

import androidx.compose.foundation.ScrollState
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.common.DEFAULT_EXERCISES
import com.example.befit.database.Exercise
import com.example.befit.database.ExerciseDao
import com.example.befit.database.Program
import com.example.befit.database.ProgramDao
import com.example.befit.database.TrainingDay
import com.example.befit.database.TrainingDayDao
import com.example.befit.database.TrainingDayExercise
import com.example.befit.database.TrainingDayExerciseDao
import com.example.befit.database.TrainingDayExerciseSet
import com.example.befit.database.TrainingDayExerciseSetDao
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class TrainingProgramsViewModel(
    private val exerciseDao: ExerciseDao,
    private val programDao: ProgramDao,
    private val trainingDayDao: TrainingDayDao,
    private val trainingDayExerciseDao: TrainingDayExerciseDao,
    private val trainingDayExerciseSetDao: TrainingDayExerciseSetDao
) : ViewModel() {

    val scrollState = mutableStateOf(ScrollState(0))

    val currentNotes = mutableStateOf("")

    fun resetScroll() {
        viewModelScope.launch {
            scrollState.value.scrollTo(0)
        }
    }

    val isEditMode = mutableStateOf(false)

    private val _currentRoute = mutableStateOf("Programs list")
    val currentRoute: String get() = _currentRoute.value

    fun updateCurrentRoute(route: String) {
        _currentRoute.value = route
    }

    private val _programs = MutableStateFlow<List<Program>>(emptyList())
    val programs: StateFlow<List<Program>> = _programs

    private val _exercises = MutableStateFlow<List<Exercise>>(emptyList())
    val exercises: StateFlow<List<Exercise>> = _exercises

    private val _trainingDays = MutableStateFlow<List<TrainingDay>>(emptyList())
    val trainingDays: StateFlow<List<TrainingDay>> = _trainingDays

    private val _trainingDaysExercises = MutableStateFlow<List<TrainingDayExercise>>(emptyList())
    val trainingDaysExercises: StateFlow<List<TrainingDayExercise>> = _trainingDaysExercises

    private val _sets = MutableStateFlow<List<TrainingDayExerciseSet>>(emptyList())
    val sets: StateFlow<List<TrainingDayExerciseSet>> = _sets

    init {
        viewModelScope.launch {
            refreshPrograms()
            refreshTrainingDays()
            refreshTrainingDaysExercises()
            refreshSets()

            refreshExercises()
            if (_exercises.value.isEmpty()) {
                for (exercise in DEFAULT_EXERCISES) {
                    exerciseDao.insert(exercise)
                }
                refreshExercises()
            }
        }
    }

    // programs

    private suspend fun refreshPrograms() {
        _programs.value = programDao.getAll()
    }

    suspend fun addProgram(name: String) {
        if (name.isNotEmpty()) {
            programDao.insert(Program(name = name))
            refreshPrograms()
        }
    }

    suspend fun updateProgram(id: Int, name: String) {
        if (name.isNotEmpty()) {
            programDao.update(Program(id = id, name = name))
            refreshPrograms()
        }
    }

    suspend fun deleteProgram(id: Int) {
        programDao.delete(Program(id = id))
        refreshPrograms()
    }

    fun getProgramById(id: Int): Program? {
        return _programs.value.find { it.id == id }
    }

    // exercises

    private suspend fun refreshExercises() {
        _exercises.value = exerciseDao.getAll().sortedBy { it.name.lowercase() }
    }

    suspend fun addExercise(name: String) {
        if (name.isNotEmpty()) {
            exerciseDao.insert(Exercise(name = name))
            refreshExercises()
        }
    }

    suspend fun updateExercise(id: Int, name: String, notes: String) {
        if (name.isNotEmpty()) {
            exerciseDao.update(Exercise(id = id, name = name, notes = notes))
            refreshExercises()
        }
    }

    suspend fun deleteExercise(id: Int) {
        exerciseDao.delete(Exercise(id = id))
        refreshExercises()
    }

    fun getExerciseById(id: Int): Exercise? {
        return _exercises.value.find { it.id == id }
    }

    // training days

    private suspend fun refreshTrainingDays() {
        _trainingDays.value = trainingDayDao.getAll()
    }

    suspend fun addTrainingDay(programId:Int, name: String) {
        if (name.isNotEmpty()) {
            trainingDayDao.insert(TrainingDay(programId = programId, name = name))
            refreshTrainingDays()
        }
    }

    suspend fun updateTrainingDay(trainingDayId: Int, programId:Int, name: String) {
        if (name.isNotEmpty()) {
            trainingDayDao.update(TrainingDay(id = trainingDayId, programId = programId, name = name))
            refreshTrainingDays()
        }
    }

    suspend fun deleteTrainingDay(id: Int) {
        trainingDayDao.delete(TrainingDay(id = id))
        refreshTrainingDays()
    }

    fun getTrainingDayById(id: Int): TrainingDay? {
        return _trainingDays.value.find { it.id == id }
    }

    // training days exercises

    private suspend fun refreshTrainingDaysExercises() {
        _trainingDaysExercises.value = trainingDayExerciseDao.getAll()
    }

    suspend fun addTrainingDayExercise(
        trainingDayId: Int,
        exerciseId: Int,
        setsNumber: Int,
        restTime: String,
        weight: Float?
    ) {
        trainingDayExerciseDao.insert(
            TrainingDayExercise(
                trainingDayId = trainingDayId,
                exerciseId = exerciseId,
                restTime = restTime.ifEmpty { null },
                weight = weight
            )
        )
        refreshTrainingDaysExercises()

        val trainingDayExercise = _trainingDaysExercises.value.find {
            it.trainingDayId == trainingDayId && it.exerciseId == exerciseId
        }

        for (i in 1..setsNumber) {
            addSet(trainingDayExercise?.id ?: 0)
        }
        refreshSets()
    }

    suspend fun updateTrainingDayExercise(
        trainingDayExerciseId: Int,
        trainingDayId: Int,
        exerciseId: Int,
        wasExerciseChanged: Boolean,
        setsNumber: Int,
        restTime: String?,
        weight: Float?
    ) {
        trainingDayExerciseDao.update(
            TrainingDayExercise(
                id = trainingDayExerciseId,
                trainingDayId = trainingDayId,
                exerciseId = exerciseId,
                restTime = restTime?.ifEmpty { null },
                weight = weight
            )
        )
        refreshTrainingDaysExercises()

        val currentSetsNumber = getNumberOfSets(trainingDayExerciseId)

        if (currentSetsNumber < setsNumber) {
            for (i in 1..setsNumber-currentSetsNumber) {
                addSet(trainingDayExerciseId)
            }
        } else if (currentSetsNumber > setsNumber) {
            val sets = getSetsFromExercise(trainingDayExerciseId)
            for (i in 1..currentSetsNumber-setsNumber) {
                deleteSet(sets[currentSetsNumber - i].id)
            }
        } else if (wasExerciseChanged) {
            val sets = getSetsFromExercise(trainingDayExerciseId)
            for (set in sets) {
                deleteSet(set.id)
            }
            for (i in 1..setsNumber) {
                addSet(trainingDayExerciseId)
            }
        }
        refreshSets()
    }

    suspend fun deleteTrainingDayExercise(id: Int) {
        val sets = getSetsFromExercise(id)
        for (set in sets) {
            deleteSet(set.id)
        }
        trainingDayExerciseDao.delete(TrainingDayExercise(id = id))
        refreshTrainingDaysExercises()
    }

    fun getTrainingDayExerciseById(id: Int): TrainingDayExercise? {
        return _trainingDaysExercises.value.find { it.id == id }
    }

    // sets

    private suspend fun refreshSets() {
        _sets.value = trainingDayExerciseSetDao.getAll()
    }

    suspend fun addSet(trainingDayExerciseId: Int) {
        trainingDayExerciseSetDao.insert(TrainingDayExerciseSet(trainingDayExerciseId = trainingDayExerciseId))
        refreshSets()
    }

    suspend fun updateSet(
        trainingDayExerciseSetId: Int,
        trainingDayExerciseId: Int,
        reps: Int?) {
        trainingDayExerciseSetDao.update(
            TrainingDayExerciseSet(
                id = trainingDayExerciseSetId,
                trainingDayExerciseId = trainingDayExerciseId,
                reps = reps
            )
        )
        refreshSets()
    }

    suspend fun deleteSet(id: Int) {
        trainingDayExerciseSetDao.delete(TrainingDayExerciseSet(id = id))
        refreshSets()
    }

    fun getSetById(id: Int): TrainingDayExerciseSet? {
        return _sets.value.find { it.id == id }
    }


    // main logic

    fun getTrainingDaysFromProgram(programId: Int): List<TrainingDay> {
        return _trainingDays.value.filter { it.programId == programId }
    }

    fun getExercisesFromTrainingDay(trainingDayId: Int): List<TrainingDayExercise> {
        return _trainingDaysExercises.value.filter { it.trainingDayId == trainingDayId }
    }

    fun getSetsFromExercise(trainingDayExerciseId: Int): List<TrainingDayExerciseSet> {
        return _sets.value.filter { it.trainingDayExerciseId == trainingDayExerciseId }
    }

    fun getExerciseFromTrainingDay(
        trainingDayId: Int,
        exerciseId: Int
    ): TrainingDayExercise? {
        return _trainingDaysExercises.value.find {
            it.trainingDayId == trainingDayId && it.exerciseId == exerciseId
        }
    }

    fun getNumberOfSets(trainingDayExerciseId: Int): Int {
        return getSetsFromExercise(trainingDayExerciseId).size
    }

    fun checkIfExerciseExists(trainingDayId: Int, exerciseId: Int): Boolean {
        return getExerciseFromTrainingDay(trainingDayId, exerciseId) != null
    }
}