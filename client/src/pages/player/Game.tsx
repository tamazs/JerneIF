import { useState } from "react";

export default function Game() {
    const [selectedNumbers, setSelectedNumbers] = useState<number[]>([]);
    const maxNumbers = 8;

    function toggleNumber(num: number) {
        if (selectedNumbers.includes(num)) {
            setSelectedNumbers(selectedNumbers.filter(n => n !== num));
        } else {
            if (selectedNumbers.length >= maxNumbers) return;
            setSelectedNumbers([...selectedNumbers, num]);
        }
    }

    async function handleSubmit() {
        console.log("Submitting numbers:", selectedNumbers);

        // example:
        // await jerneCrud.submitBoard({ userId, numbers: selectedNumbers });

        alert("Board submitted!");
    }

    function handleReset() {
        setSelectedNumbers([]);
    }

    return (
        <div className="min-h-screen flex flex-col items-center p-6 gap-6">

            <div className="text-center">
                <h1 className="text-3xl font-bold">Weekly Game #42</h1>
                <p className="opacity-70">Submit before Saturday 17:00</p>
                <p className="text-sm text-warning mt-1">
                    ⏳ 3 days 12 hours remaining
                </p>
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
                                className="px-3 py-1 bg-primary text-primary-content rounded-lg"
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
                                    ? "btn-primary text-white"
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
                disabled={selectedNumbers.length === 0}
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
        </div>
    );
}
