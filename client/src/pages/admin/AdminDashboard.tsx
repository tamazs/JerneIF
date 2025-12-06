import Card from "../../components/dashboard/Card.tsx";

export default function AdminDashboard() {
    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold mb-6">Admin Dashboard</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-6">

                    <Card
                        title="Add Game Winning Numbers"
                        description="Add the game winning numbers and create a new game"
                        linkText="Add Game Winning Numbers"
                        linkTo="/addgamewinningnumbers"
                    />

                <Card
                    title="Previous Games"
                    description="See a list of the previous games"
                    linkText="View Previous Games"
                    linkTo="/games"
                />

                <Card
                    title="Users"
                    description="See a list of all the users"
                    linkText="View users"
                    linkTo="/userslist"
                />

                    <Card
                        title="Transactions"
                        description="See a list of the recent transactions from players"
                        linkText="View Transactions"
                        linkTo="/transactions"
                    />
            </div>
        </div>
    );
}
