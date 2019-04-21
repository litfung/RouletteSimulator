﻿using Prism.Commands;
using Prism.Mvvm;
using RouletteSimulator.Core.Enumerations;
using RouletteSimulator.Core.Models.ChipModels;

namespace RouletteSimulator.Core.Models.PersonModels
{
    /// <summary>
    /// The RoulettePlayer class represents the person playing the roulette game.
    /// </summary>
    public class RoulettePlayer : BindableBase
    {
        #region Fields

        private bool _placeBets;
        private int _totalCash;
        private int _currentBet;
        private int _currentwinnings;
        private ChipType _selectedChip;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RoulettePlayer()
        {
            // Cash/bets.
            _totalCash = Constants.InitialCashDollars;
            _currentBet = 0;
            _currentwinnings = 0;

            // Chips.
            _selectedChip = ChipType.Undefined;
            OneChip = new One();
            FiveChip = new Five();
            TwentyFiveChip = new TwentyFive();
            OneHundredChip = new OneHundred();
            FiveHundredChip = new FiveHundred();
            OneThousandChip = new OneThousand();
            FiveThousandChip = new FiveThousand();
            TwentyFiveThousandChip = new TwentyFiveThousand();
            OneHundredThousandChip = new OneHundredThousand();
            FiveHundredThousandChip = new FiveHundredThousand();

            // Commands.
            ClearBetsCommand = new DelegateCommand(ClearBets).ObservesCanExecute(() => PlaceBets);
        }

        #endregion

        #region Events

        public static event ChipSelected OnChipSelected;
        public static event ClearBets OnClearBets;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the place bets status.
        /// </summary>
        public bool PlaceBets
        {
            get
            {
                return _placeBets;
            }
            set
            {
                SetProperty(ref _placeBets, value);
                CurrentWinnings = 0;    // Clear the current winnings.
            }
        }

        /// <summary>
        /// Gets the total cash held by the player.
        /// </summary>
        public int TotalCash
        {
            get
            {
                return _totalCash;
            }
            private set
            {
                SetProperty(ref _totalCash, value);
                
                // Update the selected chip.
                if (TotalCash <= 0 || TotalCash < Chip.GetChipValue(SelectedChip))
                {
                    SelectedChip = ChipType.Undefined;
                }
            }
        }

        /// <summary>
        /// Gets the current bet placed by the player.
        /// </summary>
        public int CurrentBet
        {
            get
            {
                return _currentBet;
            }
            private set
            {
                SetProperty(ref _currentBet, value);
            }
        }

        /// <summary>
        /// Gets the winnings from the current bet.
        /// </summary>
        public int CurrentWinnings
        {
            get
            {
                return _currentwinnings;
            }
            private set
            {
                SetProperty(ref _currentwinnings, value);
            }
        }

        /// <summary>
        /// Gets or sets the chip currently selected by the player.
        /// </summary>
        public ChipType SelectedChip
        {
            get
            {
                return _selectedChip;
            }
            set
            {
                if (value == ChipType.Undefined)
                {
                    // Clear the selected chip.
                    SetProperty(ref _selectedChip, value);
                    OnChipSelected?.Invoke(_selectedChip);
                }
                else if (TotalCash <= 0)
                {
                    // Player has no money - clear the selected chip.
                    SetProperty(ref _selectedChip, ChipType.Undefined);
                    OnChipSelected?.Invoke(_selectedChip);
                }
                else if (TotalCash < Chip.GetChipValue(value))
                {
                    // Player is lacking sufficient funds for the selected chip - do nothing.
                }
                else
                {
                    // Update the selected chip.
                    SetProperty(ref _selectedChip, value);
                    OnChipSelected?.Invoke(_selectedChip);
                }
            }
        }
        
        /// <summary>
        /// Gets the one dollar chip.
        /// </summary>
        public One OneChip { get; }

        /// <summary>
        /// Gets the five dollar chip.
        /// </summary>
        public Five FiveChip { get; }

        /// <summary>
        /// Gets the twenty five dollar chip.
        /// </summary>
        public TwentyFive TwentyFiveChip { get; }

        /// <summary>
        /// Gets the one hundred dollar chip.
        /// </summary>
        public OneHundred OneHundredChip { get; }

        /// <summary>
        /// Gets the five hundred dollar chip.
        /// </summary>
        public FiveHundred FiveHundredChip { get; }

        /// <summary>
        /// Gets the one thousand dollar chip.
        /// </summary>
        public OneThousand OneThousandChip { get; }

        /// <summary>
        /// Gets the five thousand dollar chip.
        /// </summary>
        public FiveThousand FiveThousandChip { get; }

        /// <summary>
        /// Gets the twenty five thousand dollar chip.
        /// </summary>
        public TwentyFiveThousand TwentyFiveThousandChip { get; }

        /// <summary>
        /// Gets the one hundred thousand dollar chip.
        /// </summary>
        public OneHundredThousand OneHundredThousandChip { get; }

        /// <summary>
        /// Gets the five hundred thousand dollar chip.
        /// </summary>
        public FiveHundredThousand FiveHundredThousandChip { get; }

        /// <summary>
        /// Gets or sets the ClearBetsCommand.
        /// </summary>
        public DelegateCommand ClearBetsCommand { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The DeductBet method is called to deduct a bet amount from the player's total cash.
        /// </summary>
        /// <param name="betAmount"></param>
        public void DeductBet(int betAmount)
        {
            if (TotalCash >= betAmount)
            {
                TotalCash = TotalCash - betAmount;
                CurrentBet = CurrentBet + betAmount;
            }
        }

        /// <summary>
        /// The ClearBets method is called to clear all bets, restoring the player's total cash.
        /// </summary>
        public void ClearBets()
        {
            if (CurrentBet > 0)
            {
                TotalCash = TotalCash + CurrentBet;
                CurrentBet = 0;
                OnClearBets?.Invoke();
            }
        }

        /// <summary>
        /// The ReceiveWinnings is called to add the winnings to the player's total.
        /// </summary>
        /// <param name="winnings"></param>
        public void ReceiveWinnings(int winnings)
        {
            CurrentWinnings = winnings <= 0 ? winnings : winnings - CurrentBet;    // Display the winnings from the current bet.

            CurrentBet = 0; // Clear the current bet.

            // Add the winnings to the total cash.
            if (winnings > 0)
            {
                TotalCash = TotalCash + winnings;
            }
            
            // TBD: If winnings are less than zero, make player sad.
            // TBD: If winnings are zero, make player mutual.
            // TBD: If winnings are greater than zero, make player happy.
        }

        #endregion
    }
}
