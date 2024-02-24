import imageRight from '../img/imageRight.jpg'; // Przykładowy obraz po prawej
import imageLeft from '../img/imageLeft.jpg'; // Przykładowy obraz po lewej

const MainPage = () => {
    return (
        <div className='text-white px-10 my-5 md:my-20 relative'>
            <h1 className='text-center text-white text-xl md:text-6xl my-10'>MebloArt - działalność rodzinna.</h1>

            {/* Sekcja z paragrafem po lewej i obrazem po prawej */}
            <div className='flex flex-col md:flex-row items-center justify-between gap-8 md:gap-10 my-10 mx-auto' style={{ maxWidth: '80%' }}>
                <div className='bg-white/30 backdrop-blur-sm rounded-lg p-4 shadow-lg'>
                    <p className='text-md text-xl'>
                        MebloArt jest to rodzinna działalność specjalizująca się w dystrybucji mebli, posiadamy magazayn w Wieluniu, z którego
                        wysyłamy meble na całą polskę!
                    </p>
                </div>
                <img className='w-full md:w-3/5 h-auto object-cover rounded-lg' src={imageRight} alt="Opis" />
            </div>

            {/* Sekcja z obrazem po lewej i paragrafem po prawej */}
            <div className='flex flex-col md:flex-row-reverse items-center justify-between gap-8 md:gap-10 my-10 mx-auto' style={{ maxWidth: '80%' }}>
                <div className='bg-white/30 backdrop-blur-sm rounded-lg p-4 shadow-lg'>
                    <p className='text-md text-xl'>
                        Firma powstała w 2000r. i poprzez prężnie rozwijającą się działaność postanowiliśmy założyć sklep dla stałych klientów!
                    </p>
                </div>
                <img className='w-full md:w-3/5 h-auto object-cover rounded-lg' src={imageLeft} alt="Opis" />
            </div>
        </div>
    );
};

export default MainPage;
