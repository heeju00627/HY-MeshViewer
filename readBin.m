clear;
clc;

fid = fopen('defaultmodel.bin', 'r');
subname = fread(fid,[1 100], 'char');
surface_flag = fread(fid, 1, 'int32');
electrode_flag = fread(fid, 1, 'int32');

% volume data
nnode = fread(fid, 1, 'int32');
nele = fread(fid, 1, 'int32');
nodes = fread(fid, [nnode 3], 'double');
elements = fread(fid, [nele 4], 'int32');
regions = fread(fid, [1 nele], 'int32');
head.nnode = nnode;
head.nele = nele;
head.nodes = nodes;
head.elements = elements;
head.regions = regions;

if( surface_flag == 1 ) 
    % scalp surface
    nnode = fread(fid, 1, 'int32'); %nnode
    nele = fread(fid, 1, 'int32'); %nele
    nodes = fread(fid, [nnode 3], 'double'); %nodes (nnode*3)
    elements = fread(fid, [nele 3], 'int32'); %elements (nele*3)
    trace = fread(fid, [1 nnode], 'int32'); %trace
    scalp.nnode = nnode;
    scalp.nele = nele;
    scalp.nodes = nodes;
    scalp.elements = elements;
    scalp.trace = trace;

    % cortical surface
    nnode = fread(fid, 1, 'int32'); %nnode
    nele = fread(fid, 1, 'int32'); %nele
    nodes = fread(fid, [nnode 3], 'double'); %nodes (nnode*3)
    elements = fread(fid, [nele 3], 'int32'); %elements (nele*3)
    trace = fread(fid, [1 nnode], 'int32'); %trace
    cortex.nnode = nnode;
    cortex.nele = nele;
    cortex.nodes = nodes;
    cortex.elements = elements;
    cortex.trace = trace;
end

if( electrode_flag == 1 )
    nelecSeed = fread(fid, 1, 'int32'); %length of list of all nodes in all electrodes
    elecSeedNodes = fread(fid, [1 nelecSeed], 'int32'); %list of all nodes in all electrodes
    nelectrode = fread(fid, 1, 'int32'); %nelectrode
    for i=1:nelectrode
        nelecInterface(i) = fread(fid, 1, 'int32'); %the number of faces between scalp and each electrode
    end
    for i=1:nelectrode
        elecInterface{i} = fread(fid, [nelecInterface(i) 4], 'int32'); %faces between scalp and each electrode (ninterface(i)*4)
                                                %'node1 node2 node3 element' - node numbers of face, element number of the face 
    end
end

fclose(fid);

%%
fid = fopen('labels.bin', 'r');
nlabels = fread(fid, 1, 'int32');

for i=1:nlabels
    name = char(fread(fid, [1 100], 'char'));
    part = char(fread(fid, [1 10], 'char'));
    nnode = fread(fid, 1, 'int32');
    nodelist = fread(fid, [1 nnode], 'int32');
    label.name = name;
    label.part = part;
    label.nnode = nnode;
    label.nodelist = nodelist;
    labels(i) = label;
end

fclose(fid);

