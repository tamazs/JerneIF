import { useState } from "react";
import useJerneCrud from "../../hooks/useJerneCrud.ts";
import type {CreateTransactionRequestDto} from "../../generated-ts-client.ts";


export default function AddTransaction() {
    const jerneCrud = useJerneCrud();
    const [addTransactionForm, setAddTransactionForm] = useState<CreateTransactionRequestDto>({
        mobilePayReference: "",
        amount: 0
    })

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        jerneCrud.addTransaction(addTransactionForm);
    };

    return (
        <div className="h-screen flex items-center justify-center">
            <div className="w-full max-w-sm p-8 rounded-xl shadow-lg">
                <h1 className="text-2xl font-bold text-center mb-6">
                    Add Transaction
                </h1>

                <form onSubmit={handleSubmit} className="space-y-4">

                    <div>
                        <label className="label">
                            <span className="label-text">MobilePay Transaction ID</span>
                        </label>
                        <input className="input validator" type="text" required placeholder="MobilePay Reference ID" onChange={(e) => setAddTransactionForm({ ...addTransactionForm, mobilePayReference: e.target.value })} />
                        <div className="validator-hint">Enter your MobilePay reference ID</div>
                    </div>

                    <div>
                        <label className="label">
                            <span className="label-text">Amount</span>
                        </label>
                        <input type="number" className="input validator" required placeholder="Amount in DKK"
                               min="1"
                               title="Must be at least 1" onChange={(e) => setAddTransactionForm({ ...addTransactionForm, amount: Number.parseInt(e.target.value)})} />
                        <div className="validator-hint">Must be at least 1</div>
                    </div>

                    <button
                        type="submit"
                        className="btn w-full rounded-lg"
                    >
                        Add Transaction
                    </button>
                </form>
            </div>
        </div>
    );
}
