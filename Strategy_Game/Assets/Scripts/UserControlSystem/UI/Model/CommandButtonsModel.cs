﻿using System;
using Abstractions.Commands;
using Abstractions.Commands.CommandsInterfaces;
using Zenject;

namespace UserControlSystem
{
    public sealed class CommandButtonsModel
    {
        public event Action<ICommandExecutor> OnCommandAccepted;
        public event Action OnCommandSent;
        public event Action OnCommandCancel;

        [Inject] private CommandCreatorBase<IProduceUnitCommand> _unitProducer;
        [Inject] private CommandCreatorBase<IAttackCommand> _attacker;
        [Inject] private CommandCreatorBase<IStopCommand> _stopper;
        [Inject] private CommandCreatorBase<IMoveCommand> _mover;
        [Inject] private CommandCreatorBase<IPatrolCommand> _patroller;

        private bool _commandIsPending;

        public void OnCommandButtonClicked(ICommandExecutor commandExecutor)
        {
            if (_commandIsPending)
            {
                processOnCancel();
            }
            _commandIsPending = true;
            OnCommandAccepted?.Invoke(commandExecutor);

            _unitProducer.ProcessCommandExecutor(commandExecutor, command => ExecuteCommandWrapper(commandExecutor, command));
            _attacker.ProcessCommandExecutor(commandExecutor, command => ExecuteCommandWrapper(commandExecutor, command));
            _stopper.ProcessCommandExecutor(commandExecutor, command => ExecuteCommandWrapper(commandExecutor, command));
            _mover.ProcessCommandExecutor(commandExecutor, command => ExecuteCommandWrapper(commandExecutor, command));
            _patroller.ProcessCommandExecutor(commandExecutor, command => ExecuteCommandWrapper(commandExecutor, command));
        }

        public void ExecuteCommandWrapper(ICommandExecutor commandExecutor, object command)
        {
            commandExecutor.ExecuteCommand(command);
            _commandIsPending = false;
            OnCommandSent?.Invoke();
        }

        public void OnSelectionChanged()
        {
            _commandIsPending = false;
            processOnCancel();
        }

        private void processOnCancel()
        {
            _unitProducer.ProcessCancel();
            _attacker.ProcessCancel();
            _stopper.ProcessCancel();
            _mover.ProcessCancel();
            _patroller.ProcessCancel();

            OnCommandCancel?.Invoke();
        }
    }
}