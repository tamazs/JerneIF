import {useEffect, useState} from "react";

import useJerneCrud from "../../hooks/useJerneCrud.ts";
import type {AddBoardRequestDto} from "../../generated-ts-client.ts";

export default function PlayerGame() {
    const jerneCrud = useJerneCrud();
    const [selectedNumbers, setSelectedNumbers] = useState<number[]>([]);
    const maxNumbers = 8;
    const minNumbers = 5;

    const [addBoard, setAddBoard] = useState<AddBoardRequestDto>({
        isRepeating: false,
        repeatCount: 0,
        boardNumbers: selectedNumbers
    })

    function toggleNumber(num: number) {
        if (selectedNumbers.includes(num)) {
            setSelectedNumbers(selectedNumbers.filter(n => n !== num));
        } else {
            if (selectedNumbers.length >= maxNumbers) return;
            setSelectedNumbers([...selectedNumbers, num]);
        }
    }

    async function handleSubmit() {

        await jerneCrud.addBoard(addBoard);

    }

    function handleReset() {
        setSelectedNumbers([]);
    }

    useEffect(() => {
        setAddBoard(prev => ({
            ...prev,
            boardNumbers: selectedNumbers
        }));
    }, [selectedNumbers]);

    return (
        <div className="min-h-screen flex flex-col items-center p-6 gap-6">

            <div className="text-center">
                <h1 className="text-3xl font-bold">Weekly Game</h1>
                <p className="opacity-70">Submit before Saturday 17:00</p>
            </div>

            <div className="w-full max-w-md bg-base-200 p-4 rounded-lg shadow">
                <h2 className="text-lg font-semibold mb-2">Your Numbers</h2>

                {selectedNumbers.length === 0 ? (
                    <p className="opacity-60">No numbers selected yet.</p>
                ) : (
                    <div className="flex flex-wrap gap-2">
                        {selectedNumbers.map(n => (
                            <span
                                key={n}
                                className="px-3 py-1 bg-secondary text-primary rounded-lg"
                            >
                                {n}
                            </span>
                        ))}
                    </div>
                )}

                <p className="text-sm opacity-70 mt-2">
                    {selectedNumbers.length} / {maxNumbers} numbers selected
                </p>
            </div>

            <div className="grid grid-cols-4 gap-3 w-full max-w-md mt-6">
                {Array.from({ length: 16 }, (_, i) => i + 1).map((num) => {
                    const isSelected = selectedNumbers.includes(num);

                    return (
                        <button
                            key={num}
                            onClick={() => toggleNumber(num)}
                            className={`btn btn-xl ${
                                isSelected
                                    ? "bg-secondary text-primary"
                                    : "btn-outline"
                            }`}
                        >
                            {num}
                        </button>
                    );
                })}
            </div>

            <button
                className="btn btn-lg btn-success w-full max-w-md mt-6"
                disabled={selectedNumbers.length < minNumbers}
                onClick={handleSubmit}
            >
                Submit Board
            </button>

            <button
                className="btn btn-lg btn-warning w-full max-w-md mt-1"
                disabled={selectedNumbers.length === 0}
                onClick={handleReset}
            >
                Reset Board
            </button>

            <label className="label mt-8">
                <span className="label-text">Repeat for more games</span>
            </label>
            <input value={addBoard.repeatCount} type="number" className="input validator" required placeholder="Repeat for number of games"
                   min="0" max="10"
                   title="Must be between be 0 to 10" onChange={(e) => {
                const value = Number.parseInt(e.target.value) || 0;

                setAddBoard(prev => ({
                    ...prev,
                    repeatCount: value,
                    isRepeating: value > 0
                }));
            }} />
        </div>
    );
}
