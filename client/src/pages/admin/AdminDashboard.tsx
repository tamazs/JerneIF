import Card from "../../components/dashboard/Card.tsx";
import AdminStat from "../../components/dashboard/admin/AdminStat.tsx";

export default function AdminDashboard() {
    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold mb-6">Admin Dashboard</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">

                <AdminStat />
                <div className="md:col-span-3">
                    <Card
                        title="Revenue"
                        description="Overview of monthly revenue"
                        linkText="View Details"
                        linkTo="/admin/revenue"
                    />
                </div>

                <Card
                    title="User Count"
                    description="Total number of registered users"
                    linkText="View Users"
                    linkTo="/admin/users"
                />

                <Card
                    title="Transactions"
                    description="Recent activity from all players"
                    linkText="Open Transactions"
                    linkTo="/admin/transactions"
                />

                    <Card
                        title="System Logs"
                        description="Everything happening behind the scenes"
                        linkText="Open Logs"
                        linkTo="/admin/logs"
                    />

                <div className="md:col-span-2">
                    <Card
                        title="Revenue"
                        description="Overview of monthly revenue"
                        linkText="View Details"
                        linkTo="/admin/revenue"
                    />
                </div>

                <Card
                    title="User Count"
                    description="Total number of registered users"
                    linkText="View Users"
                    linkTo="/admin/users"
                />

            </div>
        </div>
    );
}
