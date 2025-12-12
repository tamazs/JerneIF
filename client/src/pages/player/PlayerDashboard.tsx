import Card from "../../components/dashboard/Card.tsx";
import useJerneCrud from "../../hooks/useJerneCrud.ts";
import {useEffect} from "react";

export default function PlayerDashboard() {
    const jerneCrud = useJerneCrud();

    useEffect(() => {
        jerneCrud.getBalance()
    }, []);

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold mb-6">Player Dashboard</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-6">

                <Card
                    title="Add Board Numbers"
                    description="Add board numbers for the current game"
                    linkText="Add Board Numbers"
                    linkTo="/addboard"
                />

                <Card
                    title="Previous Boards"
                    description="See a list of your previous boards"
                    linkText="View Previous Boards"
                    linkTo="/myboards"
                />

                <Card
                    title="Add Transaction"
                    description="Update your balance by adding a transaction"
                    linkText="Add Transaction"
                    linkTo="/addtransaction"
                />

                <Card
                    title="Transactions"
                    description="See a list of your recent transactions"
                    linkText="View Transactions"
                    linkTo="/mytransactions"
                />
            </div>
        </div>
    );
}
