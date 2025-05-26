package com.example.befit.trainingprograms

import androidx.compose.foundation.ScrollState
import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.constants.DEFAULT_EXERCISES
import com.example.befit.constants.TrainingProgramsRoutes
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
import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.coroutines.launch

class TrainingProgramsViewModel(
    private val exerciseDao: ExerciseDao,
    private val programDao: ProgramDao,
    private val trainingDayDao: TrainingDayDao,
    private val trainingDayExerciseDao: TrainingDayExerciseDao,
    private val trainingDayExerciseSetDao: TrainingDayExerciseSetDao,
) : ViewModel() {
    private val _navigationEvent = MutableSharedFlow<String>()
    val navigationEvent = _navigationEvent.asSharedFlow()

    fun navigateToStart() {
        viewModelScope.launch {
            _navigationEvent.emit(TrainingProgramsRoutes.START)
        }
    }

    private val _currentRoute = mutableStateOf(TrainingProgramsRoutes.START)
    val currentRoute: State<String> = _currentRoute

    fun updateCurrentRoute(route: String) {
        if (route != _currentRoute.value) {
            _currentRoute.value = route
        }
    }

    val isEditMode = mutableStateOf(false)

    val currentNotes = mutableStateOf("")
    
    val scrollState = mutableStateOf(ScrollState(0))

    fun resetScroll() {
        viewModelScope.launch {
            scrollState.value.scrollTo(0)
        }
    }
    
    private val _programs = mutableStateOf<List<Program>>(emptyList())
    val programs: State<List<Program>> = _programs

    private val _exercises = mutableStateOf<List<Exercise>>(emptyList())
    val exercises: State<List<Exercise>> = _exercises

    private val _trainingDays = mutableStateOf<List<TrainingDay>>(emptyList())
    val trainingDays: State<List<TrainingDay>> = _trainingDays

    private val _trainingDaysExercises = mutableStateOf<List<TrainingDayExercise>>(emptyList())
    val trainingDaysExercises: State<List<TrainingDayExercise>> = _trainingDaysExercises

    private val _sets = mutableStateOf<List<TrainingDayExerciseSet>>(emptyList())
    val sets: State<List<TrainingDayExerciseSet>> = _sets

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

    fun addProgram(name: String) {
        if (name.isNotEmpty()) {
            viewModelScope.launch {
                programDao.insert(Program(name = name))
                refreshPrograms()
            }
        }
    }

    fun updateProgram(id: Int, name: String) {
        if (name.isNotEmpty()) {
            viewModelScope.launch {
                programDao.update(Program(id = id, name = name))
                refreshPrograms()
            }
        }
    }

    fun deleteProgram(id: Int) {
        viewModelScope.launch {
            programDao.delete(Program(id = id))
            refreshPrograms()
        }
    }

    // exercises

    private suspend fun refreshExercises() {
        _exercises.value = exerciseDao.getAll().sortedBy { it.name.lowercase() }
    }

    fun addExercise(name: String) {
        if (name.isNotEmpty()) {
            viewModelScope.launch {
                exerciseDao.insert(Exercise(name = name))
                refreshExercises()
            }
        }
    }

    fun updateExercise(
        id: Int,
        name: String,
        notes: String,
        videoId: Int,
    ) {
        if (name.isNotEmpty()) {
            viewModelScope.launch {
                exerciseDao.update(Exercise(id = id, name = name, notes = notes, videoId = videoId))
                refreshExercises()
            }
        }
    }

    fun deleteExercise(id: Int) {
        viewModelScope.launch {
            exerciseDao.delete(Exercise(id = id))
            refreshExercises()
        }
    }

    // training days

    private suspend fun refreshTrainingDays() {
        _trainingDays.value = trainingDayDao.getAll()
    }

    fun addTrainingDay(programId:Int, name: String) {
        if (name.isNotEmpty()) {
            viewModelScope.launch {
                trainingDayDao.insert(TrainingDay(programId = programId, name = name))
                refreshTrainingDays()
            }
        }
    }

    fun updateTrainingDay(trainingDayId: Int, programId:Int, name: String) {
        if (name.isNotEmpty()) {
            viewModelScope.launch {
                trainingDayDao.update(
                    TrainingDay(
                        id = trainingDayId,
                        programId = programId,
                        name = name
                    )
                )
                refreshTrainingDays()
            }
        }
    }

    fun deleteTrainingDay(id: Int) {
        viewModelScope.launch {
            trainingDayDao.delete(TrainingDay(id = id))
            refreshTrainingDays()
        }
    }

    // training days exercises

    private suspend fun refreshTrainingDaysExercises() {
        val data = trainingDayExerciseDao.getAll()
        _trainingDaysExercises.value = data
    }

    fun addTrainingDayExercise(
        trainingDayId: Int,
        exerciseId: Int,
        setsNumber: Int,
        restTime: String,
        weight: Float?
    ) {
        viewModelScope.launch {
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

            repeat(setsNumber) {
                addSet(trainingDayExercise?.id ?: 0)
            }
            refreshSets()
        }
    }

    fun updateTrainingDayExercise(
        trainingDayExerciseId: Int,
        trainingDayId: Int,
        exerciseId: Int,
        wasExerciseChanged: Boolean,
        setsNumber: Int,
        restTime: String?,
        weight: Float?
    ) {
        viewModelScope.launch {
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

            val currentSetsNumber = _sets.value.count { it.trainingDayExerciseId == trainingDayExerciseId }

            if (currentSetsNumber < setsNumber) {
                repeat(setsNumber - currentSetsNumber) {
                    addSet(trainingDayExerciseId)
                }
            } else if (currentSetsNumber > setsNumber) {
                val sets = _sets.value.filter { it.trainingDayExerciseId == trainingDayExerciseId }
                for (i in 1..currentSetsNumber - setsNumber) {
                    deleteSet(sets[currentSetsNumber - i].id)
                }
            } else if (wasExerciseChanged) {
                val sets = _sets.value.filter { it.trainingDayExerciseId == trainingDayExerciseId }
                for (set in sets) {
                    deleteSet(set.id)
                }
                repeat(setsNumber) {
                    addSet(trainingDayExerciseId)
                }
            }
            refreshSets()
        }
    }

    fun deleteTrainingDayExercise(id: Int) {
        viewModelScope.launch {
            val sets = _sets.value.filter { it.trainingDayExerciseId == id }
            for (set in sets) {
                deleteSet(set.id)
            }
            trainingDayExerciseDao.delete(TrainingDayExercise(id = id))
            refreshTrainingDaysExercises()
        }
    }

    // sets

    private suspend fun refreshSets() {
        _sets.value = trainingDayExerciseSetDao.getAll()
    }

    fun addSet(trainingDayExerciseId: Int) {
        viewModelScope.launch {
            trainingDayExerciseSetDao.insert(TrainingDayExerciseSet(trainingDayExerciseId = trainingDayExerciseId))
            refreshSets()
        }
    }

    fun updateSet(
        trainingDayExerciseSetId: Int,
        trainingDayExerciseId: Int,
        reps: Int?
    ) {
        viewModelScope.launch {
            trainingDayExerciseSetDao.update(
                TrainingDayExerciseSet(
                    id = trainingDayExerciseSetId,
                    trainingDayExerciseId = trainingDayExerciseId,
                    reps = reps
                )
            )
            refreshSets()
        }
    }

    fun deleteSet(id: Int) {
        viewModelScope.launch {
            trainingDayExerciseSetDao.delete(TrainingDayExerciseSet(id = id))
            refreshSets()
        }
    }

}